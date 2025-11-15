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

        public IActionResult Details(int maDatPhong)
        {
            var payment = ApiDataProvider.GetThanhToan(maDatPhong);
            if (payment is null)
            {
                return NotFound();
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

            TempData["Message"] = "Đã tạo phiếu thanh toán (demo).";
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
            if (id != model.maThanhToan)
            {
                return BadRequest();
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = "Đã cập nhật thanh toán (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var payment = ApiDataProvider.GetThanhToan(id);
            if (payment is null)
            {
                return NotFound();
            }

            TempData["Message"] = "Đã xóa phiếu thanh toán (demo).";
            return RedirectToAction(nameof(Index));
        }
    }
}
