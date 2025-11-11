using System;
using System.Linq;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var rooms = ApiDataProvider.GetPhongs();
            var bookings = ApiDataProvider.GetDatPhongs();

            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            var viewModel = new DashboardViewModel
            {
                TotalRooms = rooms.Count,
                AvailableRooms = rooms.Count(r => string.Equals(r.TinhTrang, "Trong", StringComparison.OrdinalIgnoreCase) || string.Equals(r.TinhTrang, "Trống", StringComparison.OrdinalIgnoreCase)),
                TodaysBookings = bookings.Count(dp => dp.NgayDat.Date == today),
                MonthlyRevenue = bookings.Where(dp => dp.NgayDat.Month == currentMonth && dp.NgayDat.Year == currentYear).Sum(dp => dp.TongTien),
                RecentBookings = bookings.OrderByDescending(dp => dp.NgayDat).Take(5).ToList(),
                RoomStatusBreakdown = rooms
                    .GroupBy(r => r.TinhTrang)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(viewModel);
        }
    }
}
