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
    ngayDat = DateTime.Today,
    ngayNhan = DateTime.Today,
    ngayTra = DateTime.Today.AddDays(1),
    trangThai = "DaDat",
    chiTietDatPhongs = new List<ChiTietDatPhong> { new ChiTietDatPhong() },  // ✔
    suDungDichVus = new List<SuDungDichVu> { new SuDungDichVu() },          // ✔
};


            PopulateDropdowns(model);
            ViewBag.PhongOptions = ApiDataProvider.GetPhongs();
            ViewBag.DichVuOptions = ApiDataProvider.GetDichVus();
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

            model.tongTien = CalculateTotal(model);
            TempData["Message"] = "Đã tạo đặt phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(DatPhong model)
        {
            ViewBag.KhachHangList = new SelectList(ApiDataProvider.GetKhachHangs(), "maKhachHang", "hoTen", model.maKhachHang);
            ViewBag.NhanVienList = new SelectList(ApiDataProvider.GetNhanViens(), "Id", "hoTen", model.maNhanVien);
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
                item.Selected = item.Value == model.trangThai;
            }

            ViewBag.TrangThaiList = statusItems;
        }

        private static void NormalizeCollections(DatPhong model)
        {
            model.chiTietDatPhongs = model.chiTietDatPhongs?
                .Where(ct => ct.PhongId > 0 && ct.SoDem > 0)
                .Select((ct, index) =>
                {
                    ct.Id = index + 1;
                    return ct;
                })
                .ToList();

            model.suDungDichVus = model.suDungDichVus?
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
            var roomTotal = model.chiTietDatPhongs?.Sum(ct => ct.ThanhTien) ?? 0m;
            var serviceTotal = model.suDungDichVus?.Sum(sv => sv.ThanhTien) ?? 0m;
            return roomTotal + serviceTotal;
        }
    }
}
