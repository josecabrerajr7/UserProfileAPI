using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.Http.Cors;

namespace UserProfileWebApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using UserProfileWebApi;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Profile>("Profiles");
    builder.EntitySet<Contact>("Contacts"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [EnableCors(origins: "http://localhost:9000", headers: "*", methods: "*")]
    public class ProfilesController : ODataController
    {
        private UserProfileEntities db = new UserProfileEntities();

        // GET: odata/Profiles
        [EnableQuery]
        public IQueryable<Profile> GetProfiles()
        {
            return db.Profiles;
        }

        // GET: odata/Profiles(5)
        [EnableQuery]
        public SingleResult<Profile> GetProfile([FromODataUri] int key)
        {
            return SingleResult.Create(db.Profiles.Where(profile => profile.ID == key));
        }

        // PUT: odata/Profiles(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Profile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Profile profile = db.Profiles.Find(key);
            if (profile == null)
            {
                return NotFound();
            }

            patch.Put(profile);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profile);
        }

        // POST: odata/Profiles
        public IHttpActionResult Post(Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Profiles.Add(profile);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ProfileExists(profile.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(profile);
        }

        // PATCH: odata/Profiles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Profile> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Profile profile = db.Profiles.Find(key);
            if (profile == null)
            {
                return NotFound();
            }

            patch.Patch(profile);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(profile);
        }

        // DELETE: odata/Profiles(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Profile profile = db.Profiles.Find(key);
            if (profile == null)
            {
                return NotFound();
            }

            db.Profiles.Remove(profile);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Profiles(5)/Contact
        [EnableQuery]
        public SingleResult<Contact> GetContact([FromODataUri] int key)
        {
            return SingleResult.Create(db.Profiles.Where(m => m.ID == key).Select(m => m.Contact));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileExists(int key)
        {
            return db.Profiles.Count(e => e.ID == key) > 0;
        }
    }
}
