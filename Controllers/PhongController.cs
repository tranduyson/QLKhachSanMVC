using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HotelManagement.Controllers
{
    public class PhongController : Controller
    {
        public IActionResult Index()
        {
            var phongs = ApiDataProvider.GetPhongs();
            return View(phongs);
        }

        public IActionResult Details(int id)
        {
            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                return NotFound();
            }

            return View(phong);
        }

        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Phong());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Phong model)
        {
            PopulateDropdowns(model.LoaiPhongId, model.TinhTrang);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã tạo phòng mới (demo).";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                return NotFound();
            }

            PopulateDropdowns(phong.LoaiPhongId, phong.TinhTrang);
            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Phong model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            PopulateDropdowns(model.LoaiPhongId, model.TinhTrang);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var phong = ApiDataProvider.GetPhong(id);
            if (phong is null)
            {
                return NotFound();
            }

            TempData["Message"] = "Đã xóa phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(int? selectedLoaiPhongId = null, string? selectedTinhTrang = null)
        {
            ViewBag.LoaiPhongs = ApiDataProvider.GetLoaiPhongs()
                .Select(lp => new SelectListItem
                {
                    Value = lp.Id.ToString(),
                    Text = $"{lp.TenLoaiPhong} ({lp.GiaMoiDem.ToString("N0")} đ/đêm)",
                    Selected = selectedLoaiPhongId == lp.Id
                })
                .ToList();

            var statuses = new List<SelectListItem>
            {
                new() { Value = "Trong", Text = "Trống" },
                new() { Value = "DaDat", Text = "Đã đặt" },
                new() { Value = "DangSuDung", Text = "Đang sử dụng" },
                new() { Value = "DangSua", Text = "Đang sửa" }
            };

            foreach (var item in statuses)
            {
                item.Selected = selectedTinhTrang == item.Value;
            }

            ViewBag.TinhTrangList = statuses;
        }
    }
}
