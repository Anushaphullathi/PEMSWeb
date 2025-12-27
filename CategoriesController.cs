
using Microsoft.AspNet.Identity;
using PEMS.Web.DAL.Interfaces;
using PEMS.Web.DAL.Sql;       // Categories
using PEMS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace PEMS.Web.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _repo;

        public CategoriesController()
        {
            _repo = new SqlCategoryRepository();
        }


        public CategoriesController(ICategoryRepository repo)
        {
            _repo = repo ?? new SqlCategoryRepository();
        }



        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var items = _repo.GetAll(userId).ToList();

            // Convert TempData messages into strongly-typed alerts
            var alerts = new List<AlertViewModel>();

            if (TempData.ContainsKey("Success"))
                alerts.Add(new AlertViewModel { Message = TempData["Success"]?.ToString(), Type = "success", Dismissible = true });

            if (TempData.ContainsKey("Warning"))
                alerts.Add(new AlertViewModel { Message = TempData["Warning"]?.ToString(), Type = "warning", Dismissible = true });

            if (TempData.ContainsKey("Error"))
                alerts.Add(new AlertViewModel
                {
                    Message = TempData["Error"]?.ToString(),
                    Type = "danger",
                    Dismissible = true
                });

            var vm = new CategoriesIndexViewModel
            {
                Categories = items,
                Alerts = alerts
            };

            return View(vm);
        }


        public ActionResult Create() => View(new Category());

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Category model)
        {
            var userId = User.Identity.GetUserId();

            model.UserId = userId;
            model.Name = (model.Name ?? string.Empty).Trim();

            if (_repo.ExistsByName(userId, model.Name))
                ModelState.AddModelError("Name", "A category with this name already exists.");

            if (!ModelState.IsValid) return View(model);

            var id = _repo.Create(model);
            TempData["Success"] = "Category created.";
            return RedirectToAction("Edit", new { id });
        }

        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var item = _repo.GetById(userId, id);
            if (item == null) return HttpNotFound();

            // Prevent editing global categories
            if (string.IsNullOrEmpty(item.UserId))
            {
                TempData["Warning"] = "Global categories are read-only.";
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Category model)
        {
            var userId = User.Identity.GetUserId();
            var existing = _repo.GetById(userId, model.CategoryId);
            if (existing == null) return HttpNotFound();

            if (string.IsNullOrEmpty(existing.UserId))
            {
                TempData["Warning"] = "Global categories are read-only.";
                return RedirectToAction("Index");
            }

            model.UserId = userId;
            model.Name = (model.Name ?? string.Empty).Trim();

            if (_repo.ExistsByName(userId, model.Name, excludeId: model.CategoryId))
                ModelState.AddModelError("Name", "A category with this name already exists.");

            if (!ModelState.IsValid) return View(model);

            var ok = _repo.Update(model);
            TempData["Success"] = ok ? "Updated." : "Update failed.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var item = _repo.GetById(userId, id);
            if (item == null) return HttpNotFound();

            if (string.IsNullOrEmpty(item.UserId))
            {
                TempData["Warning"] = "Global categories cannot be deleted.";
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var ok = _repo.Delete(userId, id);
            TempData["Success"] = ok ? "Deleted." : "Delete failed.";
            return RedirectToAction("Index");
        }
    }
}
