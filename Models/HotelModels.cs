namespace HotelManagement.Models
{
    public class LoaiPhong
    {
        public int Id { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public int SoGiuong { get; set; }
        public decimal? DienTich { get; set; }
        public decimal GiaMoiDem { get; set; }

    }

    public class Phong
    {
        public int Id { get; set; }
        public int MaPhong { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public LoaiPhong? LoaiPhong { get; set; }
        public string TinhTrang { get; set; } = "Trong";
    }

    public class KhachHang
    {
        public int maKhachHang { get; set; }
        public string hoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? cccd { get; set; }
        public string? ghiChu { get; set; }
        public string? DiaChi { get; set; }
    }

    public class NhanVien
    {
        public int Id { get; set; }
        public string hoTen { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? ChucVu { get; set; }
    }

    public class DichVu
    {
        public int maDichVu { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public string? ghiChu { get; set; }
        public decimal donGia { get; set; }
    }

    public class DatPhong
    {
        public int maDatPhong { get; set; }
        public int KhachHangId { get; set; }
        public KhachHang? KhachHang { get; set; }
        public int NhanVienId { get; set; }
        public NhanVien? NhanVien { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public string TrangThai { get; set; } = "DaDat";
        public decimal TongTien { get; set; }
        public List<ChiTietDatPhong>? ChiTietDatPhongs { get; set; }
        public List<SuDungDichVu>? SuDungDichVus { get; set; }
    }

    public class ChiTietDatPhong
    {
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public DatPhong? DatPhong { get; set; }
        public int PhongId { get; set; }
        public Phong? Phong { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
        public decimal ThanhTien => DonGia * SoDem;
    }

    public class SuDungDichVu
    {
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public DatPhong? DatPhong { get; set; }
        public int DichVuId { get; set; }
        public DichVu? DichVu { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }

    public class ThanhToan
    {
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public DatPhong? DatPhong { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string PhuongThuc { get; set; } = "TienMat";
        public string TrangThai { get; set; } = "DaThanhToan";
    }

    // Request models for API
    public class DatPhongRequest
    {
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public List<ChiTietDatPhongRequest> ChiTietDatPhongs { get; set; } = new();
        public List<SuDungDichVuRequest> SuDungDichVus { get; set; } = new();
    }

    public class ChiTietDatPhongRequest
    {
        public int PhongId { get; set; }
        public decimal DonGia { get; set; }
        public int SoDem { get; set; }
    }

    public class SuDungDichVuRequest
    {
        public int DichVuId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}
