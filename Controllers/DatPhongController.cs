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
            var model = new DatPhongRequest
            {
                NgayDat = DateTime.Today,
                NgayNhan = DateTime.Today,
                NgayTra = DateTime.Today.AddDays(1),
                TrangThai = "DaDat",
                ChiTietDatPhongs = new List<ChiTietDatPhongRequest>
                    {
                        new ChiTietDatPhongRequest
                        {
                        }
                    },
                            SuDungDichVus = new List<SuDungDichVuRequest>
                    {
                        new SuDungDichVuRequest
                        {
                        }
                    }
            };

            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromBody] DatPhongRequest model)
        {
            // ✅ Guard kỹ hơn
            if (model == null)
            {
                model = new DatPhongRequest();
            }
            model.ChiTietDatPhongs ??= new List<ChiTietDatPhongRequest>();
            model.SuDungDichVus ??= new List<SuDungDichVuRequest>();

            // ✅ Loại bỏ các phần tử null trong collection
            model.ChiTietDatPhongs = model.ChiTietDatPhongs
                .Where(ct => ct != null)
                .ToList();
            model.SuDungDichVus = model.SuDungDichVus
                .Where(sv => sv != null)
                .ToList();

            NormalizeCollections(model);
            PopulateDropdowns(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // ✅ Chuyển đổi từ DatPhong model sang DatPhongRequest DTO
                var request = new DatPhongRequest
                {
                    MaKhachHang = model.MaKhachHang,
                    MaNhanVien = model.MaNhanVien,
                    NgayDat = model.NgayDat,
                    NgayNhan = model.NgayNhan,
                    NgayTra = model.NgayTra,
                    TrangThai = model.TrangThai ?? "Đã đặt",
                    ChiTietDatPhongs = model.ChiTietDatPhongs
                        .Where(ct => ct != null)
                        .Select(ct => new ChiTietDatPhongRequest
                        {
                            MaPhong = ct.MaPhong,
                            DonGia = ct.DonGia,
                            SoDem = ct.SoDem // Hoặc SoDem nếu có property này
                        }).ToList(),
                    SuDungDichVus = model.SuDungDichVus
                        .Where(sv => sv != null)
                        .Select(sv => new SuDungDichVuRequest
                        {
                            MaDichVu = sv.MaDichVu,
                            SoLuong = sv.SoLuong,
                            DonGia = sv.DonGia
                        }).ToList()
                };

                // ✅ Gọi API để tạo đặt phòng
                var createdDatPhong = ApiDataProvider.CreateDatPhong(request);

                if (createdDatPhong != null)
                {
                    TempData["SuccessMessage"] = "Đặt phòng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo đặt phòng. Vui lòng thử lại.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi đặt phòng: {ex.Message}";
                return View(model);
            }
        }

        private void PopulateDropdowns(DatPhongRequest model)
        {
            var khachHangs = ApiDataProvider.GetKhachHangs() ?? Enumerable.Empty<KhachHang>();
            var nhanViens = ApiDataProvider.GetNhanViens() ?? Enumerable.Empty<NhanVien>();
            var phongs = ApiDataProvider.GetPhongs() ?? Enumerable.Empty<Phong>();
            var dichVus = ApiDataProvider.GetDichVus() ?? Enumerable.Empty<DichVu>();
            var loaiPhongs = ApiDataProvider.GetLoaiPhongs() ?? Enumerable.Empty<LoaiPhong>();

            ViewBag.KhachHangList = new SelectList(khachHangs, "maKhachHang", "hoTen", model.MaKhachHang);
            ViewBag.NhanVienList = new SelectList(nhanViens, "Id", "hoTen", model.MaNhanVien);
            ViewBag.PhongOptions =  new SelectList(phongs, "MaPhong", "SoPhong",null);
            ViewBag.DichVuOptions = new SelectList(dichVus, "MaDichVu", "TenDichVu", null);
            ViewBag.LoaiPhongOptions = new SelectList(loaiPhongs, "MaLoaiPhong", "TenLoaiPhong", null);
        }

        private static void NormalizeCollections(DatPhongRequest model)
        {
            model.ChiTietDatPhongs = model.ChiTietDatPhongs?
                .Where(ct => ct.MaPhong > 0 && ct.SoDem > 0)
                .Select((ct, index) =>
                {
                    ct.MaPhong = index + 1;
                    return ct;
                })
                .ToList();

            model.SuDungDichVus = model.SuDungDichVus?
                .Where(sv => sv.MaDichVu > 0 && sv.SoLuong > 0)
                .Select((sv, index) =>
                {
                    sv.MaDichVu = index + 1;
                    return sv;
                })
                .ToList();
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

            try
            {
                var deleted = ApiDataProvider.DeleteDatPhong(id);
                if (deleted)
                {
                    TempData["SuccessMessage"] = "Đã xóa đặt phòng.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Xóa thất bại. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
            }

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
