using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace HotelManagement.Models
{
    public static class ApiDataProvider
    {
        private static readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        static ApiDataProvider()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7158/")
            };
        }

        private static T? GetFromApi<T>(string relativePath)
        {
            try
            {
                var resp = _httpClient.GetAsync(relativePath).GetAwaiter().GetResult();
                if (!resp.IsSuccessStatusCode) return default;
                var json = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch(Exception ex)
            {
                // API unreachable or deserialization failed -> return default (null)
                return default;
            }
        }

        public static IReadOnlyList<LoaiPhong> GetLoaiPhongs()
            => GetFromApi<List<LoaiPhong>>("api/LoaiPhong") ?? new List<LoaiPhong>();

        public static LoaiPhong? GetLoaiPhong(int id)
            => GetFromApi<LoaiPhong>($"api/LoaiPhong/{id}");

        public static IReadOnlyList<Phong> GetPhongs()
            => GetFromApi<List<Phong>>("api/Phong") ?? new List<Phong>();

        public static Phong? GetPhong(int id)
            => GetFromApi<Phong>($"api/Phong/{id}");

        public static IReadOnlyList<KhachHang> GetKhachHangs()
            => GetFromApi<List<KhachHang>>("api/KhachHang") ?? new List<KhachHang>();

        public static KhachHang? GetKhachHang(int id)
            => GetFromApi<KhachHang>($"api/KhachHang/{id}");

        public static IReadOnlyList<NhanVien> GetNhanViens()
            => GetFromApi<List<NhanVien>>("api/NhanVien") ?? new List<NhanVien>();

        public static NhanVien? GetNhanVien(int id)
            => GetFromApi<NhanVien>($"api/NhanVien/{id}");

        public static IReadOnlyList<DichVu> GetDichVus()
            => GetFromApi<List<DichVu>>("api/DichVu") ?? new List<DichVu>();

        public static DichVu? GetDichVu(int id)
            => GetFromApi<DichVu>($"api/DichVu/{id}");

        public static IReadOnlyList<DatPhong> GetDatPhongs()
            => GetFromApi<List<DatPhong>>("api/DatPhong") ?? new List<DatPhong>();

        public static DatPhong? GetDatPhong(int id)
            => GetFromApi<DatPhong>($"api/DatPhong/{id}");

        public static IReadOnlyList<ThanhToan> GetThanhToans()
            => GetFromApi<List<ThanhToan>>("api/ThanhToan") ?? new List<ThanhToan>();

        public static ThanhToan? GetThanhToan(int id)
            => GetFromApi<ThanhToan>($"api/ThanhToan/{id}");
    }
}
