using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class DichVuController : Controller
    {
        public IActionResult Index()
        {
            var dichVus = ApiDataProvider.GetDichVus();
            return View(dichVus);
        }

        public IActionResult Details(int id)
        {
            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                return NotFound();
            }

            return View(dichVu);
        }

        public IActionResult Create()
        {
            return View(new DichVu());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DichVu model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã thêm dịch vụ (demo).";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                return NotFound();
            }

            return View(dichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DichVu model)
        {
            if (id != model.maDichVu)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật dịch vụ (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var dichVu = ApiDataProvider.GetDichVu(id);
            if (dichVu is null)
            {
                return NotFound();
            }

            TempData["Message"] = "Đã xóa dịch vụ (demo).";
            return RedirectToAction(nameof(Index));
        }
    }
}
