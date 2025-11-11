using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class LoaiPhongController : Controller
    {
        public IActionResult Index()
        {
            var loaiPhongs = ApiDataProvider.GetLoaiPhongs();
            return View(loaiPhongs);
        }

        public IActionResult Details(int id)
        {
            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                return NotFound();
            }

            return View(loaiPhong);
        }

        public IActionResult Create()
        {
            return View(new LoaiPhong());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoaiPhong model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã tạo loại phòng mới (demo).";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                return NotFound();
            }

            return View(loaiPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LoaiPhong model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật loại phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var loaiPhong = ApiDataProvider.GetLoaiPhong(id);
            if (loaiPhong is null)
            {
                return NotFound();
            }

            TempData["Message"] = "Đã xóa loại phòng (demo).";
            return RedirectToAction(nameof(Index));
        }
    }
}
