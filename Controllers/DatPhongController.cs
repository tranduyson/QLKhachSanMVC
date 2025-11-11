using System;
using System.Collections.Generic;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelManagement.Controllers
{
    public class DatPhongController : Controller
    {
        public IActionResult Index()
        {
            var bookings = ApiDataProvider.GetDatPhongs();
            return View(bookings);
        }

        public IActionResult Details(int id)
        {
            var booking = ApiDataProvider.GetDatPhong(id);
            if (booking is null)
            {
                return NotFound();
            }

            return View(booking);
        }

        public IActionResult Create()
        {
            var model = new DatPhong
            {
                NgayDat = DateTime.Today,
                NgayNhan = DateTime.Today,
                NgayTra = DateTime.Today.AddDays(1),
                TrangThai = "DaDat",
                ChiTietDatPhongs = new List<ChiTietDatPhong> { new(), new() },
                SuDungDichVus = new List<SuDungDichVu> { new() }
            };

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DatPhong model)
        {
            NormalizeCollections(model);
            PopulateDropdowns(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TongTien = CalculateTotal(model);
            TempData["Message"] = "Đã tạo đặt phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(DatPhong model)
        {
            ViewBag.KhachHangList = new SelectList(ApiDataProvider.GetKhachHangs(), "Id", "TenKhachHang", model.KhachHangId);
            ViewBag.NhanVienList = new SelectList(ApiDataProvider.GetNhanViens(), "Id", "TenNhanVien", model.NhanVienId);
            ViewBag.PhongOptions = ApiDataProvider.GetPhongs();
            ViewBag.DichVuOptions = ApiDataProvider.GetDichVus();

            var statusItems = new List<SelectListItem>
            {
                new() { Value = "DaDat", Text = "Đã đặt" },
                new() { Value = "DaNhan", Text = "Đã nhận" },
                new() { Value = "DaTra", Text = "Đã trả" },
                new() { Value = "Huy", Text = "Hủy" }
            };

            foreach (var item in statusItems)
            {
                item.Selected = item.Value == model.TrangThai;
            }

            ViewBag.TrangThaiList = statusItems;
        }

        private static void NormalizeCollections(DatPhong model)
        {
            model.ChiTietDatPhongs = model.ChiTietDatPhongs?
                .Where(ct => ct.PhongId > 0 && ct.SoDem > 0)
                .Select((ct, index) =>
                {
                    ct.Id = index + 1;
                    return ct;
                })
                .ToList();

            model.SuDungDichVus = model.SuDungDichVus?
                .Where(sv => sv.DichVuId > 0 && sv.SoLuong > 0)
                .Select((sv, index) =>
                {
                    sv.Id = index + 1;
                    return sv;
                })
                .ToList();
        }

        private static decimal CalculateTotal(DatPhong model)
        {
            var roomTotal = model.ChiTietDatPhongs?.Sum(ct => ct.ThanhTien) ?? 0m;
            var serviceTotal = model.SuDungDichVus?.Sum(sv => sv.ThanhTien) ?? 0m;
            return roomTotal + serviceTotal;
        }
    }
}
