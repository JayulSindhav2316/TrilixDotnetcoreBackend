using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Max.Services.Helpers
{
    public class GetBillableEntity
    {
        public static string GetBillableName(Entity entity)
        {
            string result = "";
            if (entity != null)
            {
                if (entity.CompanyId != null)
                {
                    result = entity.Companies.FirstOrDefault(x => x.CompanyId == x.Entity.CompanyId)?.CompanyName;
                    return result;
                }
                else if (entity.PersonId != null)
                {
                    var data = entity.People.FirstOrDefault(x => x.PersonId == x.Entity.PersonId);
                    if (data != null)
                    {
                        result = data.Prefix + " " + data.FirstName + " " + data.LastName;
                        return result;
                    }
                }
            }
            return result;
        }
    }
}
