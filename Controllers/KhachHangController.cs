using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class KhachHangController : Controller
    {
        public IActionResult Index()
        {
            var khachHangs = ApiDataProvider.GetKhachHangs();
            return View(khachHangs);
        }

        public IActionResult Details(int id)
        {
            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        public IActionResult Create()
        {
            return View(new KhachHang());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã thêm khách hàng (demo).";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, KhachHang model)
        {
            if (id != model.maKhachHang)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật khách hàng (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var khachHang = ApiDataProvider.GetKhachHang(id);
            if (khachHang is null)
            {
                return NotFound();
            }

            TempData["Message"] = "Đã xóa khách hàng (demo).";
            return RedirectToAction(nameof(Index));
        }
    }
}
