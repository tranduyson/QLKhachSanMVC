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
            var payments = ApiDataProvider.GetThanhToans();
            return View(payments);
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
                TrangThai = "DaThanhToan"
            };

            PopulateBookingOptions(model.DatPhongId, model.TrangThai);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ThanhToan model)
        {
            PopulateBookingOptions(model.DatPhongId, model.TrangThai);

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

            PopulateBookingOptions(payment.DatPhongId, payment.TrangThai);
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

            PopulateBookingOptions(model.DatPhongId, model.TrangThai);

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

        private void PopulateBookingOptions(int selectedBookingId, string? selectedStatus)
        {
            var bookingItems = ApiDataProvider.GetDatPhongs()
                .Select(dp => new SelectListItem
                {
                    Value = dp.maDatPhong.ToString(),
                    Text = $"#{dp.maDatPhong} - {dp.KhachHang?.hoTen} ({dp.NgayNhan:dd/MM} - {dp.NgayTra:dd/MM})",
                    Selected = dp.maDatPhong == selectedBookingId
                })
                .ToList();

            ViewBag.DatPhongList = bookingItems;

            var statusItems = new List<SelectListItem>
            {
                new() { Value = "DaThanhToan", Text = "Đã thanh toán" },
                new() { Value = "ChuaThanhToan", Text = "Chưa thanh toán" },
                new() { Value = "HoanTien", Text = "Hoàn tiền" }
            };

            foreach (var item in statusItems)
            {
                item.Selected = item.Value == selectedStatus;
            }

            ViewBag.TrangThaiList = statusItems;
        }
    }
}
