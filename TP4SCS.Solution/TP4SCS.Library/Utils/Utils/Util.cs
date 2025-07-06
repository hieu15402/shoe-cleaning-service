using System.Text;
using TP4SCS.Library.Utils.StaticClass;

namespace TP4SCS.Library.Utils.Utils
{
    public class Util
    {
        public Util() { }

        // Password-related methods
        public string CheckPasswordErrorType(string password)
        {
            bool hasDigit = false;
            bool hasLower = false;
            bool hasUpper = false;
            bool hasSpecial = false;

            foreach (char ch in password)
            {
                if (char.IsDigit(ch)) hasDigit = true;
                else if (char.IsLower(ch)) hasLower = true;
                else if (char.IsUpper(ch)) hasUpper = true;
                else if (!char.IsLetterOrDigit(ch)) hasSpecial = true;

                if (hasDigit && hasLower && hasUpper && hasSpecial)
                {
                    return "None";
                }
            }

            if (!hasDigit) return "Number";
            if (!hasLower) return "Lower";
            if (!hasUpper) return "Upper";
            if (!hasSpecial) return "Special";

            return "None";
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool CompareHashedPassword(string password1, string password2)
        {
            return BCrypt.Net.BCrypt.Verify(password1, password2);
        }

        // String utility methods
        public static string UpperCaseStringStatic(string input)
        {
            return input.Trim().ToUpperInvariant();
        }

        public static bool IsEqual(string s1, string s2)
        {
            return string.Equals(s1.Trim(), s2.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // Status-related methods
        public bool CheckStatusForAdmin(string status, string statusRequest)
        {
            return !string.Equals(status, statusRequest, StringComparison.OrdinalIgnoreCase);
        }

        public static string TranslateGeneralStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "Trạng Thái Không Hợp Lệ";

            var lowerStatus = status.Trim().ToLowerInvariant();
            return lowerStatus switch
            {
                "unavailable" => "Ngưng Hoạt Động",
                "available" => "Hoạt Động",
                _ => "Trạng Thái Không Hợp Lệ"
            };
        }

        public static string TranslateOrderStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "Trạng Thái Không Hợp Lệ";

            var lowerStatus = status.Trim().ToLowerInvariant();
            return lowerStatus switch
            {
                "canceled" => "Đã hủy",
                "pending" => "Đang chờ",
                "approved" => "Đã xác nhận",
                "received" => "Đã nhận",
                "processing" => "Đang xử lý",
                "storage" => "Lưu trữ",
                "shipping" => "Đang giao hàng",
                "delivered" => "Đã giao hàng",
                "finished" => "Hoàn thành",
                "abandoned" => "Quá hạn nhận hàng",
                _ => "Trạng Thái Không Hợp Lệ"
            };
        }

        public static string TranslateOrderDetailStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "Trạng Thái Không Hợp Lệ";

            var lowerStatus = status.Trim().ToLowerInvariant();
            return lowerStatus switch
            {
                "pending" => "Đang chờ",
                "received" => "Đã nhận",
                "processing" => "Đang xử lý",
                "done" => "Hoàn thành",
                _ => "Trạng Thái Không Hợp Lệ"
            };
        }
        public static string TranslateBranchStatus(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "Trạng Thái Không Hợp Lệ";

            var lowerStatus = status.Trim().ToLowerInvariant();
            return lowerStatus switch
            {
                "active" => "Hoạt động",
                "inactive" => "Không Hoạt Động",
                "suspended" => "Bị Đình Chỉ",
                _ => "Trạng Thái Không Hợp Lệ"
            };
        }
        public static bool IsValidGeneralStatus(string status)
        {
            var validStatuses = new[] { StatusConstants.AVAILABLE, StatusConstants.UNAVAILABLE };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidOrderStatus(string status)
        {
            var validStatuses = new[]
            {
                StatusConstants.CANCELED,
                StatusConstants.PENDING,
                StatusConstants.APPROVED,
                StatusConstants.RECEIVED,
                StatusConstants.PROCESSING,
                StatusConstants.STORAGE,
                StatusConstants.SHIPPING,
                StatusConstants.DELIVERED,
                StatusConstants.FINISHED,
                StatusConstants.ABANDONED
            };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsValidOrderDetailStatus(string status)
        {
            var validStatuses = new[]
            {
                StatusConstants.PENDING,
                StatusConstants.RECEIVED,
                StatusConstants.PROCESSING,
                StatusConstants.DONE
            };
            return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
        }

        // Account-related methods
        public string TranslateAccountStatus(string status)
        {
            return status switch
            {
                "INACTIVE" => "Ngưng Hoạt Động",
                "SUSPENDED" => "Đã Khoá",
                _ => "Hoạt Động"
            };
        }

        public string TranslateAccountRole(string role)
        {
            return role switch
            {
                "OWNER" => "Chủ Cung Cấp",
                "EMPLOYEE" => "Nhân Viên",
                "ADMIN" => "Quản Trị Viên",
                "MODERATOR" => "Người Điều Hành",
                _ => "Khách Hàng"
            };
        }

        public bool CheckAccountRole(string role)
        {
            return role.Trim().ToUpperInvariant() switch
            {
                "OWNER" => true,
                "EMPLOYEE" => true,
                "ADMIN" => true,
                "MODERATOR" => true,
                "CUSTOMER" => true,
                _ => false
            };
        }

        // Branch-related methods
        public bool CheckBranchStatus(string status)
        {
            return status.Trim().ToUpperInvariant() switch
            {
                "ACTIVE" => true,
                "INACTIVE" => true,
                "SUSPENDED" => true,
                _ => false
            };
        }

        // Employee-related methods
        public string CheckDeleteEmployeesErrorType(string? old, List<int> input)
        {
            if (string.IsNullOrEmpty(old)) return "Empty";

            var oldList = old.Split(",").Select(int.Parse).ToList();
            var oldSet = new HashSet<int>(oldList);

            foreach (var element in input)
            {
                if (!oldSet.Contains(element))
                {
                    return "Null";
                }
            }
            return "None";
        }
        public static List<int> ConvertStringToList(string str)
        {
            return str.Split(',')
                      .Select(int.Parse)
                      .ToList();
        }
        public static string ConvertListToString(List<int> numbers)
        {
            return string.Join(",", numbers);
        }

        public string CheckAddEmployeesErrorType(string? old, List<int> input)
        {
            var oldList = new List<int>();

            if (!string.IsNullOrEmpty(old))
            {
                oldList = old.Split(',').Select(int.Parse).ToList();
            }

            if (oldList.Count > 5) return "Full";

            var oldSet = new HashSet<int>(oldList);
            foreach (var element in input)
            {
                if (oldSet.Contains(element))
                {
                    return "Existed";
                }
            }
            return "None";
        }

        public string DeleteEmployeesId(string? old, List<int> input)
        {
            var oldList = string.IsNullOrEmpty(old) ? new List<int>() : old.Split(',').Select(int.Parse).ToList();
            foreach (var element in input)
            {
                oldList.Remove(element);
            }
            return string.Join(",", oldList);
        }

        public string AddEmployeeId(string? old, List<int> input)
        {
            var oldList = string.IsNullOrEmpty(old) ? new List<int>() : old.Split(',').Select(int.Parse).ToList();
            foreach (var element in input)
            {
                oldList.Add(element);
            }
            return string.Join(",", oldList);
        }

        public string FormatStringName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            StringBuilder result = new StringBuilder(name.Length);
            bool newWord = true;

            foreach (char c in name)
            {
                if (char.IsWhiteSpace(c))
                {
                    result.Append(c);
                    newWord = true;
                }
                else
                {
                    result.Append(newWord ? char.ToUpper(c) : char.ToLower(c));
                    newWord = false;
                }
            }

            return result.ToString();
        }

        public bool CheckTicketStatus(string status)
        {
            return status.Trim().ToUpperInvariant() switch
            {
                StatusConstants.OPENING => true,
                StatusConstants.PROCESSING => true,
                StatusConstants.RESOLVING => true,
                StatusConstants.CLOSED => true,
                _ => false,
            };
        }

        public bool CheckStatus(string status)
        {
            bool result = status.Trim().ToUpperInvariant() switch
            {
                "ACTIVE" => true,
                "INACTIVE" => true,
                "UNREGISTERED" => true,
                "EXPIRED" => true,
                _ => false
            };

            return result;
        }

        public bool CheckStatusForAdmin(string status)
        {
            bool result = status.Trim().ToUpperInvariant() switch
            {
                "ACTIVE" => true,
                "INACTIVE" => true,
                "SUSPENDED" => true,
                _ => false
            };

            return result;
        }

        public bool CheckDateTime(DateTime register, DateTime expired)
        {
            if (register >= expired) return false;

            return true;
        }
    }
}
