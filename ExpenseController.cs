
using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PEMS.Web.DAL.Interfaces;
using PEMS.Web.DAL.Sql;         // for quick new-up if not using DI
using PEMS.Web.Models;

namespace PEMS.Web.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ITransactionRepository _transactions;
        private readonly ICategoryRepository _categories;

        // Quick new-up (no DI). Later we can switch to DI.
        public ExpenseController()
        {
            _transactions = new SqlTransactionRepository();
            _categories = new SqlCategoryRepository();
        }

        // Index: list with filters
        public ActionResult Index(DateTime? from, DateTime? to, int? categoryId, decimal? min, decimal? max, string paymentMethod)
        {
            var userId = User.Identity.GetUserId();
            var filter = new TransactionFilter
            {
                UserId = userId,
                TransactionType = "Expense",
                From = from,
                To = to,
                CategoryId = categoryId,
                MinAmount = min,
                MaxAmount = max,
                PaymentMethod = paymentMethod
            };

            var items = _transactions.GetTransactions(filter);
            ViewBag.Categories = new SelectList(_categories.GetAll(userId), "CategoryId", "Name");
            return View(items);
        }

        // Create: form
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Categories = new SelectList(_categories.GetAll(userId), "CategoryId", "Name");
            return View(new Transaction { TxnDate = DateTime.Today, TransactionType = "Expense" });
        }

        // Create: submit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Transaction model)
        {
            var userId = User.Identity.GetUserId();

            model.UserId = userId;
            model.TransactionType = "Expense";

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categories.GetAll(userId), "CategoryId", "Name");
                return View(model);
            }

            var id = _transactions.Create(model);
            TempData["Success"] = "Expense recorded.";
            return RedirectToAction("Index");
        }

        // Edit: form
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var item = _transactions.GetById(userId, id);
            if (item == null || item.TransactionType != "Expense") return HttpNotFound();

            ViewBag.Categories = new SelectList(_categories.GetAll(userId), "CategoryId", "Name", item.CategoryId);
            return View(item);
        }

        // Edit: submit
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Transaction model)
        {
            var userId = User.Identity.GetUserId();
            var existing = _transactions.GetById(userId, model.TransactionId);
            if (existing == null || existing.TransactionType != "Expense") return HttpNotFound();

            model.UserId = userId;
            model.TransactionType = "Expense";

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categories.GetAll(userId), "CategoryId", "Name", model.CategoryId);
                return View(model);
            }

            var ok = _transactions.Update(model);
            TempData["Success"] = ok ? "Updated." : "Update failed.";
            return RedirectToAction("Index");
        }

        // Delete: confirm
        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var item = _transactions.GetById(userId, id);
            if (item == null || item.TransactionType != "Expense") return HttpNotFound();
            return View(item);
        }

        // Delete: submit
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var ok = _transactions.Delete(userId, id);
            TempData["Success"] = ok ? "Deleted." : "Delete failed.";
            return RedirectToAction("Index");
        }
    }
}
