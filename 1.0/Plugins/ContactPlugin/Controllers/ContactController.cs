using ContactPlugin.Models;
using DAL.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ContactPlugin.Controllers
{
    public class ContactController : Common.Controllers.BaseController
    {
        public ActionResult Index()
        {   
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ContactFormBasic model)
        {
            processContactFormBasic(model);
            return View(model);
        }

        public ActionResult BasicForm()
        {
            return PartialView("BasicForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BasicForm(ContactFormBasic model)
        {
            model = processContactFormBasic(model);
            return PartialView("BasicForm", model);
        }

        private ContactFormBasic processContactFormBasic(ContactFormBasic model)
        {
            if (ModelState.IsValid)
            {
                var c = new ContactForm();
                c.Name = model.Name;
                c.Email = model.EmailAddress;
                c.PhoneNumber = model.PhoneNumber;
                c.Message = model.Message;
                c.CreatedDateUtc = DateTime.UtcNow;
                c.Insert();
                model = new ContactFormBasic();
                ModelState.Clear();
                AddSuccess("The contact form was successfully submitted");
            }
            return model;
        }
    }
}