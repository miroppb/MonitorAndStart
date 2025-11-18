using Dapper;
using Dapper.Contrib.Extensions;
using miroppb;
using MonitorAndStart.v2.Models;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Data
{
	public interface IMainDataProvider
	{
		Task<IEnumerable<Job>?> GetJobsAsync();
		int InsertJob(Job jobs);
		bool UpdateJob(Job job);
		Task<bool> DeleteJob(Job job, int workflowId);
		Task<(IEnumerable<Workflow>?, IEnumerable<Job>?)> GetWorkflowsAndJobsAsync();
		Task<Workflow>? GetWorkflowAsync(int id, List<Job> AllJobs);
		int InsertWorkflow(Workflow workflow);
		bool UpdateWorkflow(Workflow workflow);
		bool DeleteWorkflow(Workflow workflow, bool UnusedJobs);
		Task<Settings?> GetSettings();
		Task<Settings?> SaveSettings(Settings? currentSettings);
	}

	public class MainDataProvider : IMainDataProvider
	{
		public async Task<IEnumerable<Job>?> GetJobsAsync()
		{
			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				IEnumerable<TempJob> temp = await db.QueryAsync<TempJob>($"SELECT * FROM jobs WHERE pcname = @MachineName", new { Environment.MachineName });
				return MapToJobs(temp);
			}
			catch { return null; }
		}

		public int InsertJob(Job job)
		{
			TempJob tempJob = MapToTempJob(job);
			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				Libmiroppb.Log($"Inserting into Database: {JsonConvert.SerializeObject(tempJob)}");
				return (int)db.Insert(tempJob);
			}
			catch { }

			return -1;
		}

		public bool UpdateJob(Job job)
		{
			TempJob tempJob = MapToTempJob(job);

			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				db.Update(tempJob);
				return true;
			}
			catch { }

			return false;
		}

		public async Task<bool> DeleteJob(Job job, int workflowId)
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			try
			{
				//lets check Workflows, to make sure that the job isn't being used
				var otherWorkflowsWithJob = db.Query<int>($"SELECT id FROM workflows WHERE jobIDs LIKE '%{job.Id}%'");
				var ex = otherWorkflowsWithJob.Except([workflowId]).ToList();
				Workflow.Loading = true;
				var workflow = db.QueryFirstOrDefault<Workflow>("SELECT * FROM workflows WHERE id = @workflowId", new { workflowId });
				if (workflow != null)
				{
					//remove the job from list of jobs
					List<int> jobs = [.. workflow.JobIDs.Split(",").Select(int.Parse)];
					jobs.Remove(job.Id);
					workflow.JobIDs = string.Join(',', jobs);
					await db.UpdateAsync(workflow);
				}
				Workflow.Loading = false;
				if (ex.Count == 0) //this is the only workflow with this job
				{
					TempJob tempJob = MapToTempJob(job);
					await db.DeleteAsync(tempJob);
					db.Execute($"ALTER TABLE jobs AUTO_INCREMENT = 0;"); //this will automatically reset to the next available number
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public async Task<(IEnumerable<Workflow>?, IEnumerable<Job>?)> GetWorkflowsAndJobsAsync()
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			Workflow.Loading = true;
			var temp = await db.QueryMultipleAsync("SELECT * FROM workflows WHERE pcname = @MachineName; SELECT * FROM jobs WHERE pcname = @MachineName", new { Environment.MachineName });
			var work = await temp.ReadAsync<Workflow>();
			var jobs = await temp.ReadAsync<TempJob>();
			var retJobs = MapToJobs(jobs);
			foreach (var wf in work)
			{
				if (wf.JobIDs != string.Empty)
				{
					List<int> jobIds = [.. wf.JobIDs.Split(',').Select(int.Parse)];
					wf.Jobs = jobIds.Select(id => retJobs!.FirstOrDefault(x => x.Id == id)).Where(job => job != null).ToList()!;
				}
			}

			Workflow.Loading = false;
			return (work, retJobs);
		}

		public async Task<Workflow>? GetWorkflowAsync(int id, List<Job> AllJobs)
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			Workflow.Loading = true;
			var temp = await db.QueryFirstOrDefaultAsync<Workflow>("SELECT * FROM workflows WHERE pcname = @MachineName AND id = @id", new { Environment.MachineName, id }); //FUTURE: make single call
			if (temp != null)
			{
				if (temp.JobIDs != string.Empty)
				{
					List<int> jobIds = temp.JobIDs.Split(',').Select(int.Parse).ToList();
					temp.Jobs = AllJobs!.Where(x => jobIds.Contains(x.Id)).ToList();
					return temp;
				}
			}
			Workflow.Loading = false;
			return new Workflow();
		}

		public int InsertWorkflow(Workflow workflow)
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			workflow.PcName = Environment.MachineName;
			workflow.JobIDs = string.Join(",", workflow.Jobs.Select(x => x.Id).ToList());
			return (int)db.Insert(workflow);
		}

		public bool UpdateWorkflow(Workflow workflow)
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			workflow.JobIDs = string.Join(",", workflow.Jobs.Select(x => x.Id).ToList());
			return db.Update(workflow);
		}

		public bool DeleteWorkflow(Workflow workflow, bool UnusedJobs)
		{
			using MySqlConnection db = Secrets.GetConnectionString();

			//check if there's any other workflows that use the same jobs. If so, don't remove them
			if (UnusedJobs)
			{
				var jobIdArray = workflow.JobIDs.Split(',');

				var otherWorkflowJobIds = jobIdArray.Where(jobId =>
					db.Query<int>($"SELECT COUNT(*) FROM workflows WHERE id <> @id AND jobIDs LIKE '%{jobId}%'",
					new { id = workflow.Id }).Single() > 0
				).ToList();

				var nonOverlappingJobIds = string.Join(",", jobIdArray.Except(otherWorkflowJobIds));
				if (nonOverlappingJobIds.Length > 0)
					db.Execute("DELETE FROM jobs WHERE id IN (@jobIds)", new { jobIds = nonOverlappingJobIds });
			}

			return db.Delete(workflow);
		}

		private static List<Job> MapToJobs(IEnumerable<TempJob> temp)
		{
			List<Job> result = [];
			foreach (TempJob tempJob in temp)
			{
				switch (tempJob.Type)
				{
					case 0:
						FileJson fj = JsonConvert.DeserializeObject<FileJson>(tempJob.Json)!;
						result.Add(new File(tempJob.Name, fj.filename, fj.parameters, fj.restart, fj.runasadmin, fj.runonce, fj.consoleapp)
						{
							Id = tempJob.Id,
							Enabled = tempJob.Enabled
						});
						break;
					case 1:
						ServiceJson sj = JsonConvert.DeserializeObject<ServiceJson>(tempJob.Json)!;
						result.Add(new Service(tempJob.Name, sj.servicename)
						{
							Id = tempJob.Id,
							Enabled = tempJob.Enabled
						});
						break;
					case 2:
						StuckJson stj = JsonConvert.DeserializeObject<StuckJson>(tempJob.Json)!;
						result.Add(new Stuck(tempJob.Name, stj.filename, stj.stucklongerthanminutes)
						{
							Id = tempJob.Id,
							Enabled = tempJob.Enabled
						});
						break;
					case 3:
						ScriptJson scj = JsonConvert.DeserializeObject<ScriptJson>(tempJob.Json)!;
						result.Add(new Script(tempJob.Name, scj.filename, scj.parameters, scj.runasadmin, scj.runhidden, scj.runonce)
						{
							Id = tempJob.Id,
							Enabled = tempJob.Enabled
						});
						break;
					case 4:
						APIJson apj = JsonConvert.DeserializeObject<APIJson>(tempJob.Json)!;
						result.Add(new API(tempJob.Name, apj.url, apj.cookies, apj.output, apj.notifyonfailurebutcomplete)
						{
							Id = tempJob.Id,
							Enabled = tempJob.Enabled
						});
						break;
                    case 5:
                        PauseJson paj = JsonConvert.DeserializeObject<PauseJson>(tempJob.Json)!;
                        result.Add(new Pause(tempJob.Name, paj.seconds)
                        {
                            Id = tempJob.Id,
                            Enabled = tempJob.Enabled
                        });
                        break;
                }
			}
			return result;
		}

		private static TempJob MapToTempJob(Job job) //dont forget to map
		{
			TempJob tempJob = new() { Id = job.Id, Name = job.Name, Type = job.TypeOfJob, PcName = Environment.MachineName, Enabled = job.Enabled };
			if (job is File file)
			{
				FileJson js = new() { filename = file.filename, parameters = file.parameters, restart = file.restart, runasadmin = file.runAsAdmin, runonce = file.runOnce, consoleapp = file.consoleApp };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}
			else if (job is Service service)
			{
				ServiceJson js = new() { servicename = service.ServiceName };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}
			else if (job is Stuck stuck)
			{
				StuckJson js = new() { filename = stuck.Filename, stucklongerthanminutes = stuck.StuckLongerThanMinutes };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}
			else if (job is Script script)
			{
				ScriptJson js = new() { filename = script.filename, parameters = script.parameters, runasadmin = script.runAsAdmin, runhidden = script.runHidden, runonce = script.runOnce };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}
			else if (job is API api)
			{
				APIJson js = new() { url = api.url, cookies = api.cookies, output = api.output, notifyonfailurebutcomplete = api.NotifyOnFailureButComplete };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}
			else if (job is Pause pause)
			{
				PauseJson js = new() { seconds = pause.seconds };
				tempJob.Json= JsonConvert.SerializeObject(js);
			}
			return tempJob;
		}

		public async Task<Settings?> GetSettings()
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			return await db.QueryFirstOrDefaultAsync<Settings>("SELECT * FROM settings WHERE pcname = @MachineName", new { Environment.MachineName });
		}

		public async Task<Settings?> SaveSettings(Settings? currentSettings)
		{
			using MySqlConnection db = Secrets.GetConnectionString();
			var exists = await db.QueryFirstOrDefaultAsync<Settings>("SELECT * FROM settings WHERE pcname = @MachineName", new { Environment.MachineName });
			if (exists != null)
				db.Update(currentSettings);
			else
				db.Insert(currentSettings);
			return await db.QueryFirstOrDefaultAsync<Settings>("SELECT * FROM settings WHERE pcname = @MachineName", new { Environment.MachineName });
		}
	}
}
