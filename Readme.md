Tài liệu tổng hợp: Farming Game
1. Tổng quan
Tựa game: Farming Simulator Loại game: Nông trại mô phỏng (Farming Simulation) Nền tảng: PC (Unity) Ngôn ngữ lập trình: C#

Game mô phỏng cuộc sống nông trại với các hoạt động như trồng trọt, nấu ăn, nuôi thú cưng và trải nghiệm các mùa vụ trong năm. Game tích hợp công nghệ AI Gemini để tạo tính năng dự báo thời tiết và gợi ý công thức nấu ăn.

2. Hệ thống chính
2.1. Hệ thống thời gian (TimeController)
Chu kỳ ngày/đêm với thời gian có thể điều chỉnh
Theo dõi ngày hiện tại và chuyển sang ngày mới
Tốc độ thời gian có thể điều chỉnh (timeSpeed)
2.2. Hệ thống mùa vụ (SeasonSystem)
4 mùa: Xuân, Hạ, Thu, Đông
Mỗi mùa có ảnh hưởng khác nhau đến cây trồng:
Xuân: Cây cối phát triển bình thường
Hạ: Cà chua và ngô phát triển nhanh hơn, nhưng cần nhiều nước
Thu: Cà rốt và các loại củ phát triển nhanh hơn
Đông: Cây cối phát triển chậm, một số cây không thể trồng
2.3. Hệ thống thời tiết (WeatherSystem)
Các loại thời tiết: Quang đãng, Mưa, Bão, Sương mù
Hiệu ứng thời tiết: mưa, bão, tuyết (particle effects)
Dự báo thời tiết sử dụng AI Gemini
2.4. Hệ thống trồng trọt (CropController)
Trồng nhiều loại cây khác nhau
Quản lý tưới nước và phát triển cây trồng
Ảnh hưởng theo mùa và thời tiết
2.5. Hệ thống nấu ăn (CookingSystem)
Danh sách công thức nấu ăn
Gợi ý công thức dựa trên nguyên liệu hiện có (sử dụng AI Gemini)
Hiệu ứng tăng cường từ thức ăn (hồi phục stamina, tăng tốc độ, giảm tiêu hao thể lực)
2.6. Hệ thống thú cưng (PetSystem)
Tương tác với thú cưng
Hệ thống tình cảm (affection)
Thú cưng có thể tìm đồ và theo người chơi
3. Cơ chế gameplay
3.1. Công cụ (Tools)
Nhiều loại công cụ: cày (plough), bình tưới (wateringCan), hạt giống (seeds), giỏ (basket)
Mỗi công cụ tiêu hao stamina khi sử dụng
3.2. Quản lý thể lực (Stamina)
Hệ thống stamina giới hạn hoạt động
Tự phục hồi theo thời gian
Thức ăn và nghỉ ngơi giúp hồi phục nhanh hơn
3.3. Kinh tế
Bán sản phẩm và kiếm tiền
Mua hạt giống, công cụ và vật phẩm
4. Tính năng người chơi
4.1. Tương tác môi trường
Chuyển đổi giữa trong nhà và ngoài trời
Tương tác với các đối tượng trong thế giới
4.2. Hệ thống inventory
Quản lý vật phẩm (crops, tools, seeds)
UI hiển thị chi tiết vật phẩm
4.3. Kỹ năng nấu ăn
Chế biến nguyên liệu thành món ăn
Công thức nấu ăn với hiệu ứng khác nhau
5. Tính năng AI và trí tuệ nhân tạo
5.1. Dự báo thời tiết (GeminiAPIClient)
Sử dụng Google Gemini API để tạo dự báo thời tiết chi tiết
Hiển thị dự báo khi bắt đầu ngày mới
5.2. Gợi ý công thức nấu ăn
Phân tích nguyên liệu hiện có và đề xuất công thức có thể nấu
Tạo mô tả và hướng dẫn nấu ăn
5.3. Hệ thống thú cưng thông minh
Phản hồi tương tác với người chơi
Khả năng tự khám phá môi trường
6. UI và trải nghiệm người dùng
6.1. Bảng điều khiển chính
Hiển thị thời gian, ngày, mùa
Thanh stamina và tiền
6.2. Hệ thống menu
Inventory
Cooking panel
Pet menu
Shop
6.3. Thông báo
Hệ thống message panel hiển thị thông báo
Dự báo thời tiết và thông tin quan trọng
7. Hệ thống lưu trữ và tiến độ
7.1. SaveManager
Lưu và tải tiến độ chơi
Bảo tồn trạng thái thế giới và người chơi
8. Cấu trúc mã nguồn
8.1. Kiến trúc hệ thống
Singleton pattern cho các manager chính
Component-based design cho nhân vật và đối tượng
8.2. Tích hợp API
GeminiAPIClient cho giao tiếp với Google Gemini API
Xử lý không đồng bộ (async/await) cho API calls
8.3. Scene Management
Chuyển đổi giữa các scene (Main, House, DayEnd)
SceneTransitionManager để xử lý chuyển cảnh
9. Hướng phát triển tương lai
9.1. Cải thiện tiềm năng
Thêm loại cây trồng và công thức nấu ăn
Mở rộng hệ thống thú cưng với nhiều loài
Thêm NPC và tương tác xã hội
9.2. Tối ưu hóa
Cải thiện hiệu suất cho các hiệu ứng thời tiết
Tối ưu hóa việc gọi API
9.3. Tính năng mới
Hệ thống chăn nuôi
Khai thác mỏ và tài nguyên
Chế tạo đồ đạc và trang trí nhà cửa
10. Hướng dẫn triển khai
10.1. Cài đặt môi trường
Unity version tương thích
Cấu hình API keys cho Gemini
10.2. Tổng quan cấu trúc thư mục
Assets/Scripts: Mã nguồn C#
Assets/Prefabs: Đối tượng prefab
Assets/Resources: Tài nguyên như sprites, âm thanh
Assets/Animations: Hoạt ảnh cho nhân vật và đối tượng
Lưu ý: Tài liệu này được tổng hợp dựa trên mã nguồn và thông tin hiện có. Một số tính năng có thể đang trong quá trình phát triển.