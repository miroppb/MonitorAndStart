using Dapper;
using Dapper.Contrib.Extensions;
using miroppb;
using MonitorAndStart.v2.Models;
using MySqlConnector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonitorAndStart.v2.Data
{
	public interface IMainDataProvider
	{
		Task<IEnumerable<Job>?> GetJobsAsync();
		bool InsertRecord(Job jobs);
		bool UpdateRecord(Job job);
		bool DeleteRecord(Job job);
	}

	public class MainDataProvider : IMainDataProvider
	{
		public async Task<IEnumerable<Job>?> GetJobsAsync()
		{
			libmiroppb.Log($"Get List of Jobs");
			IEnumerable<Job> jobs = new List<Job>();
			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				IEnumerable<TempJob> temp = await db.QueryAsync<TempJob>($"SELECT * FROM jobs");
				List<Job> result = new();
				foreach (TempJob tempJob in temp)
				{
					switch (tempJob.Type)
					{
						case 0:
							FileJson fj = JsonConvert.DeserializeObject<FileJson>(tempJob.Json)!;
							result.Add(new File(tempJob.Name, fj.filename, fj.parameters, fj.restart, fj.runasadmin, tempJob.Intervalinminutes,
								tempJob.Selectedinterval, tempJob.Lastrun, tempJob.Nexttimetorun, tempJob.RunOnStart)
							{
								Id = tempJob.Id
							});
							break;
						case 1:
							ServiceJson sj = JsonConvert.DeserializeObject<ServiceJson>(tempJob.Json)!;
							result.Add(new Service(tempJob.Name, sj.servicename, tempJob.Intervalinminutes, tempJob.Selectedinterval,
								tempJob.Lastrun, tempJob.Nexttimetorun, tempJob.RunOnStart)
							{
								Id = tempJob.Id
							});
							break;
						case 2:
							StuckJson stj = JsonConvert.DeserializeObject<StuckJson>(tempJob.Json)!;
							result.Add(new Stuck(tempJob.Name, stj.filename, stj.stucklongerthanminutes, tempJob.Intervalinminutes, tempJob.Selectedinterval,
								tempJob.Lastrun, tempJob.Nexttimetorun, tempJob.RunOnStart)
							{
								Id = tempJob.Id
							});
							break;
						case 3:
							ScriptJson scj = JsonConvert.DeserializeObject<ScriptJson>(tempJob.Json)!;
							result.Add(new Script(tempJob.Name, scj.filename, scj.parameters, scj.runasadmin, scj.runhidden, tempJob.Intervalinminutes, tempJob.Selectedinterval,
								tempJob.Lastrun, tempJob.Nexttimetorun, tempJob.RunOnStart)
							{
								Id = tempJob.Id
							});
							break;
					}
				}
				jobs = result;
			}
			catch { }
			return jobs;
		}

		public bool InsertRecord(Job job)
		{
			TempJob tempJob = MapJobToTempJob(job);
			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				libmiroppb.Log($"Inserting into Database: {JsonConvert.SerializeObject(tempJob)}");
				job.Id = (int)db.Insert(tempJob);
				return true;
			}
			catch { }

			return false;
		}

		public bool UpdateRecord(Job job)
		{
			TempJob tempJob = MapJobToTempJob(job);

			try
			{
				using MySqlConnection db = Secrets.GetConnectionString();
				db.Update(tempJob);
				return true;
			}
			catch { }

			return false;
		}

		private static TempJob MapJobToTempJob(Job job)
		{
			TempJob tempJob = new() { Id = job.Id, Name = job.Name, Type = job.TypeOfJob, Intervalinminutes = job.IntervalInMinutes, Selectedinterval = job.Interval, Lastrun = job.LastRun, Nexttimetorun = job.NextTimeToRun, RunOnStart = job.RunOnStart };
			if (job is File file)
			{
				FileJson js = new() { filename = file.filename, parameters = file.parameters, restart = file.restart, runasadmin = file.runAsAdmin };
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
				ScriptJson js = new() { filename = script.Filename, parameters = script.Parameters, runasadmin = script.RunAsAdmin, runhidden = script.RunHidden };
				tempJob.Json = JsonConvert.SerializeObject(js);
			}

			return tempJob;
		}

		public bool DeleteRecord(Job job)
		{
			TempJob tempJob = MapJobToTempJob(job);
			using MySqlConnection db = Secrets.GetConnectionString();
			return db.Delete(tempJob);
		}
	}
}
