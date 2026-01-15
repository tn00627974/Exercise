using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDojo.Fundamentals
{
    public class ProductController
    {
        public ActionResult GetProduct(int id)
        {
            try 
            {
                if (id == 0)
                    return new NotFound();
                
                if (id < 0)
                    throw new ArgumentException("ID cannot be negative");
            }
            catch (Exception)
            {
                return new BadRequest();
            }

            return new Ok();
        }
    }

    public class ActionResult { public virtual int GetStatusCode() => 200; }

    public class NotFound : ActionResult { public override int GetStatusCode() => 404; }

    public class Ok : ActionResult { public override int GetStatusCode() => 200; }

    // 500 
    public class BadRequest : ActionResult { public override int GetStatusCode() => 500; }

}
