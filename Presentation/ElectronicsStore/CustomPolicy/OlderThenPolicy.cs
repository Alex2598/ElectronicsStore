using Microsoft.AspNetCore.Authorization;

namespace Store.Web.CustomPolicy
{
	public class OlderThenPolicy : IAuthorizationRequirement
	{
		public int Age { get; set; }
		public OlderThenPolicy(int age) 
		{ 
			Age = age;
		}
	}
}
