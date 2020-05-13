using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.UI.MVC.Models; //aded for models
using System.Net.Mail;//added for .Net Email (MailMessage, smtpClient)
using System.Net;//added for network credential

namespace StoreFront.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() //get action - gets us to the form
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//verify no-man-in-the-middle attack. 
        public ActionResult Contact(ContactForm cfm) //post action - processes the form
        {
            if (!ModelState.IsValid)
            {
                //bad data, did not follow metadata requirements. send them back to view with error messages to try again.
                return View(cfm);
            }

            //if ex gets this far the model metadata was good. 

            //we will make a mail message for our email to send out. body will be the longest and most complicated part, lets pre-build that.
            string message = $"Email From {cfm.Name} with a suject of {cfm.Subject}.<br/>" +
                $"Please respond to {cfm.Email} with your thoughts. Message below...<br>" +
                $"<blockquote>{cfm.Message}</blockquote><em>Generated {DateTime.Now}</em>";


            //setup mailmessage object

            MailMessage mm = new MailMessage("postmaster@jamesgriffithdev.com", "james.griffith3@outlook.com", cfm.Subject, message);

            //some commonly used options for MailMessage:
            mm.IsBodyHtml = true;//needed if there's embedded html in body
            mm.Priority = MailPriority.High; //sets flag for important
            mm.ReplyToList.Add(cfm.Email);//adds the actual sender of the form their email address. hitting reply does actually go back to that email address.(otherwise it would only go back to the no-reply address.

            //smtp is the "send email" protocol. object below represents the system we have credentials for that will send our email from our FROM-address.
            SmtpClient emailer = new SmtpClient("mail.jamesgriffithdev.com");

            //client credentials (SmarterASP requies your username & password)
            emailer.Credentials = new NetworkCredential("postmaster@jamesgriffithdev.com", "!James9601");

            try
            {
                //attempt to send email
                emailer.Send(mm);
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = "Sorry, something went wrong. Please try again later or contact sys admin.<br/>" +
                    $"Error msg: <blockquote>{ex.StackTrace}</blockquote>";
                return View(cfm);//send them back with error info. well have to call on vb variable.
            }//"Dangerous code" it has a dependency so it woulc fail ie: the mail server being down.

            ////v1 quick success confirm: just pass back a vb variable to show on the same page.    
            //ViewBag.Confirm = "Hey, that worked! we'll hitcha back later.....thaaanks";

            //return View();



            //v2 success confirm on its own new page
            //make a new view and return them there...


            return View("MessageConfirm", cfm);//generate this view with details scaffolding (readonly 1 obj)


        }


        public ActionResult Products()
        {
            ViewBag.Message = "Description";
            ViewBag.Title = "Products Page";
            return View();
        }
    }
}