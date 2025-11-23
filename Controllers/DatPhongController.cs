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
                chiTietDatPhongs = new List<ChiTietDatPhong>
        {
            new ChiTietDatPhong
            {
                SoPhong = string.Empty,
                TenLoaiPhong = string.Empty
            }
        },
                suDungDichVus = new List<SuDungDichVu>
        {
            new SuDungDichVu
            {
                TenDichVu = string.Empty
            }
        }
            };

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DatPhong model)
        {
            // ✅ Guard kỹ hơn
            if (model == null)
            {
                model = new DatPhong();
            }

            model.chiTietDatPhongs ??= new List<ChiTietDatPhong>();
            model.suDungDichVus ??= new List<SuDungDichVu>();

            // ✅ Loại bỏ các phần tử null trong collection
            model.chiTietDatPhongs = model.chiTietDatPhongs
                .Where(ct => ct != null)
                .ToList();

            model.suDungDichVus = model.suDungDichVus
                .Where(sv => sv != null)
                .ToList();

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
            var khachHangs = ApiDataProvider.GetKhachHangs() ?? Enumerable.Empty<KhachHang>();
            var nhanViens = ApiDataProvider.GetNhanViens() ?? Enumerable.Empty<NhanVien>();
            var phongs = ApiDataProvider.GetPhongs() ?? Enumerable.Empty<Phong>();
            var dichVus = ApiDataProvider.GetDichVus() ?? Enumerable.Empty<DichVu>();

            ViewBag.KhachHangList = new SelectList(khachHangs, "maKhachHang", "hoTen", model.maKhachHang);
            ViewBag.NhanVienList = new SelectList(nhanViens, "Id", "hoTen", model.maNhanVien);
            ViewBag.PhongOptions =  new SelectList(phongs, "MaPhong", "SoPhong",null);
            ViewBag.DichVuOptions = new SelectList(dichVus, "MaDichVu", "TenDichVu", null);
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

        public IActionResult Edit(int id)
        {
            var booking = ApiDataProvider.GetDatPhong(id);
            if (booking == null)
            {
                return NotFound();
            }

            PopulateDropdowns(booking);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DatPhong model)
        {
            if (id != model.maDatPhong)
            {
                return NotFound();
            }

            model.chiTietDatPhongs ??= new List<ChiTietDatPhong>();
            model.suDungDichVus ??= new List<SuDungDichVu>();

            // Remove null elements
            model.chiTietDatPhongs = model.chiTietDatPhongs
                .Where(ct => ct != null)
                .ToList();

            model.suDungDichVus = model.suDungDichVus
                .Where(sv => sv != null)
                .ToList();

            NormalizeCollections(model);
            PopulateDropdowns(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.tongTien = CalculateTotal(model);
            TempData["Message"] = "Đã cập nhật đặt phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var booking = ApiDataProvider.GetDatPhong(id);
            if (booking == null)
            {
                return NotFound();
            }

            // In a real implementation, you would delete from the database
            TempData["Message"] = "Đã xóa đặt phòng (demo).";
            return RedirectToAction(nameof(Index));
        }

        private static decimal CalculateTotal(DatPhong model)
        {
            var roomTotal = model.chiTietDatPhongs?.Sum(ct => ct.ThanhTien) ?? 0m;
            var serviceTotal = model.suDungDichVus?.Sum(sv => sv.ThanhTien) ?? 0m;
            return roomTotal + serviceTotal;
        }
    }
}
