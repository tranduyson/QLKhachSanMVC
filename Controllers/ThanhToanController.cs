using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.Controllers
{
    public class ThanhToanController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var payments = ApiDataProvider.GetThanhToans();
                return View(payments);
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                TempData["Error"] = "Không tìm thấy thanh toán này.";
                return RedirectToAction(nameof(Index));
            }

            return View(payment);
        }

        public IActionResult Create()
        {
            var model = new ThanhToan
            {
                NgayThanhToan = DateTime.Today,
                TrangThai = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ThanhToan model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure new record id = 0
            model.maThanhToan = 0;

            var created = ApiDataProvider.CreateThanhToan(model);
            if (created is null)
            {
                TempData["Error"] = "Không thể tạo thanh toán. Vui lòng kiểm tra API.";
                return View(model);
            }

            TempData["Message"] = "Đã tạo phiếu thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                return NotFound();
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ThanhToan model)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.maThanhToan)
            {
                TempData["Error"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = ApiDataProvider.UpdateThanhToan(model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật thanh toán. Vui lòng thử lại.";
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                TempData["Error"] = "Không tìm thấy thanh toán này.";
                return RedirectToAction(nameof(Index));
            }

            var success = ApiDataProvider.DeleteThanhToan(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa thanh toán. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Đã xóa thanh toán thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
