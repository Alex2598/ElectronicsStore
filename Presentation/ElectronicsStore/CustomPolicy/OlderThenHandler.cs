using Microsoft.AspNetCore.Authorization;

using System.Globalization;

namespace Store.Web.CustomPolicy
{
	public class OlderThenHandler : AuthorizationHandler<OlderThenPolicy>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OlderThenPolicy requirement)
		{
			var claim = context.User.Claims.Where(claim => claim.Type == "Birthday").FirstOrDefault();
			if (claim != null)
			{
				var date = DateTime.ParseExact(claim.Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
				var age = DateTime.Now.Year - date.Year;
				if (DateTime.Now.DayOfYear < date.DayOfYear)
					age++;
                if (age>=requirement.Age) 
				{
					context.Succeed(requirement);
				}
				else
					context.Fail();
			}
			else
				context.Fail();
			return Task.CompletedTask;
		}
	}
}
