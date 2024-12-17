using MonitorAndStart.v2.Models;
using System;
using System.Linq;

namespace MonitorAndStart.v2.Validation
{
	public static class ValidateJobExtension
	{
		public static (bool, string) Validate(this Job job)
		{
			if (job == null)
				return (false, "Job cannot be null");

			switch (job)
			{
				case API api:
					if (api.cookies.Length > 0)
					{
						// Validate each cookie format
						string[] cookies = api.cookies.Split(';');
						foreach (string cookie in cookies)
						{
							if (!IsValidCookie(cookie))
							{
								return (false, $"Invalid cookie format: {cookie}{Environment.NewLine}Needs to be name=value;name=value");
							}
						}
						return (true, "API job is valid");
					}
					else
						return (true, "");

				// Add cases for other Job implementations if needed
				default:
					return (false, "Unknown job type");
			}
		}

		private static bool IsValidCookie(string cookie)
		{
			// Check if cookie is in "name=value" format and both parts are non-empty
			var parts = cookie.Split('=');
			parts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
			return parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]);
		}
	}
}
