using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;

namespace UserProfileWebApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using UserProfileWebApi;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Contact>("Contacts");
    builder.EntitySet<Profile>("Profiles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ContactsController : ODataController
    {
        private UserProfileEntities db = new UserProfileEntities();

        // GET: odata/Contacts
        [EnableQuery]
        public IQueryable<Contact> GetContacts()
        {
            return db.Contacts;
        }

        // GET: odata/Contacts(5)
        [EnableQuery]
        public SingleResult<Contact> GetContact([FromODataUri] string key)
        {
            return SingleResult.Create(db.Contacts.Where(contact => contact.ContactID == key));
        }

        // PUT: odata/Contacts(5)
        public IHttpActionResult Put([FromODataUri] string key, Delta<Contact> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contact contact = db.Contacts.Find(key);
            if (contact == null)
            {
                return NotFound();
            }

            patch.Put(contact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(contact);
        }

        // POST: odata/Contacts
        public IHttpActionResult Post(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Contacts.Add(contact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ContactExists(contact.ContactID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(contact);
        }

        // PATCH: odata/Contacts(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] string key, Delta<Contact> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Contact contact = db.Contacts.Find(key);
            if (contact == null)
            {
                return NotFound();
            }

            patch.Patch(contact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(contact);
        }

        // DELETE: odata/Contacts(5)
        public IHttpActionResult Delete([FromODataUri] string key)
        {
            Contact contact = db.Contacts.Find(key);
            if (contact == null)
            {
                return NotFound();
            }

            db.Contacts.Remove(contact);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Contacts(5)/Profiles
        [EnableQuery]
        public IQueryable<Profile> GetProfiles([FromODataUri] string key)
        {
            return db.Contacts.Where(m => m.ContactID == key).SelectMany(m => m.Profiles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContactExists(string key)
        {
            return db.Contacts.Count(e => e.ContactID == key) > 0;
        }
    }
}
