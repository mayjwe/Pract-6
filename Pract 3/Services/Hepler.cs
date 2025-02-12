using Pract_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract_3
{
    internal class Helper
    {
        private static AtelierEntities _context;

        public static AtelierEntities GetContext()
        {
            return new AtelierEntities();
        }
        public void CreateClients(Clients client)
        {
            try
            {
                var context = GetContext();
                if (context == null)
                {
                    throw new InvalidOperationException("Database context is not initialized.");
                }

                context.Clients.Add(client);
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var validationErrors in e.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
        }

    }
}
