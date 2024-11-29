using Pract_3.Models;
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
            if (_context == null)
            {
                _context = new AtelierEntities();
            }
            return _context;
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

        private void UpdateClients(Clients client)
        {
            _context.Entry(client).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        private void RemoveClients(int ID_Clients)
        {
            var client = _context.Clients.Find(ID_Clients);
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }

        private List<Clients> FiltrClients()
        {
            return _context.Clients.Where(x => x.Name.StartsWith("M") || x.Name.StartsWith("A")).ToList();
        }

        private List<Clients> SortClients()
        {
            return _context.Clients.OrderBy(x => x.Name).ToList();
        }

        private string GetClientTypes(Clients client)
        {
            return client.Name.GetType().ToString();
        }
    }
}
