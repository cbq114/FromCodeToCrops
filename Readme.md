# Tài Liệu Tổng Hợp: Farming Game

## 1. Tổng Quan
- **Tựa game**: Farming Simulator  
- **Loại game**: Nông trại mô phỏng (Farming Simulation)  
- **Nền tảng**: PC (Unity)  
- **Ngôn ngữ lập trình**: C#  

Game mô phỏng cuộc sống nông trại với các hoạt động chính như trồng trọt, nấu ăn, nuôi thú cưng và trải nghiệm các mùa vụ trong năm. Game tích hợp công nghệ AI Gemini để hỗ trợ tính năng dự báo thời tiết và gợi ý công thức nấu ăn, mang lại trải nghiệm độc đáo và chân thực cho người chơi.

---

## 2. Hệ Thống Chính

### 2.1. Hệ Thống Thời Gian (TimeController)
- Chu kỳ ngày/đêm với thời gian có thể điều chỉnh linh hoạt.  
- Theo dõi ngày hiện tại và tự động chuyển sang ngày mới khi chu kỳ hoàn tất.  
- Tốc độ thời gian có thể tùy chỉnh thông qua tham số `timeSpeed`.  

### 2.2. Hệ Thống Mùa Vụ (SeasonSystem)
- Bao gồm 4 mùa: **Xuân**, **Hạ**, **Thu**, **Đông**.  
- Mỗi mùa ảnh hưởng khác nhau đến sự phát triển của cây trồng:  
  - **Xuân**: Cây cối phát triển bình thường, tốc độ ổn định.  
  - **Hạ**: Cà chua và ngô phát triển nhanh hơn, nhưng yêu cầu lượng nước tưới nhiều hơn.  
  - **Thu**: Cà rốt và các loại củ có tốc độ phát triển nhanh hơn.  
  - **Đông**: Cây cối phát triển chậm, một số loại cây không thể trồng được.  

### 2.3. Hệ Thống Thời Tiết (WeatherSystem)
- Các loại thời tiết: **Quang đãng**, **Mưa**, **Bão**, **Sương mù**.  
- Hiệu ứng thời tiết được thể hiện qua particle effects (mưa, bão, tuyết).  
- Tích hợp dự báo thời tiết sử dụng công nghệ AI Gemini, cung cấp thông tin chi tiết cho người chơi.  

### 2.4. Hệ Thống Trồng Trọt (CropController)
- Hỗ trợ trồng nhiều loại cây khác nhau.  
- Quản lý quá trình tưới nước và theo dõi sự phát triển của cây trồng.  
- Sự phát triển chịu ảnh hưởng từ mùa vụ và thời tiết hiện tại.  

### 2.5. Hệ Thống Nấu Ăn (CookingSystem)
- Danh sách công thức nấu ăn đa dạng.  
- Gợi ý công thức dựa trên nguyên liệu hiện có trong kho, sử dụng AI Gemini để phân tích và đề xuất.  
- Thức ăn mang lại hiệu ứng tăng cường như:  
  - Hồi phục stamina.  
  - Tăng tốc độ di chuyển.  
  - Giảm tiêu hao thể lực khi làm việc.  

### 2.6. Hệ Thống Thú Cưng (PetSystem)
- Tương tác với thú cưng thông qua các hành động như vuốt ve, cho ăn.  
- Hệ thống tình cảm (`affection`) đo lường mức độ gắn bó với thú cưng.  
- Thú cưng có khả năng tìm kiếm vật phẩm và đi theo người chơi trong game.  

---

## 3. Cơ Chế Gameplay

### 3.1. Công Cụ (Tools)
- Các loại công cụ bao gồm: **cày (plough)**, **bình tưới (wateringCan)**, **hạt giống (seeds)**, **giỏ (basket)**.  
- Mỗi công cụ tiêu hao một lượng stamina nhất định khi sử dụng.  

### 3.2. Quản Lý Thể Lực (Stamina)
- Hệ thống stamina giới hạn số lượng hoạt động người chơi có thể thực hiện.  
- Stamina tự phục hồi dần theo thời gian.  
- Sử dụng thức ăn hoặc nghỉ ngơi giúp hồi phục stamina nhanh hơn.  

### 3.3. Kinh Tế
- Bán sản phẩm thu hoạch để kiếm tiền trong game.  
- Dùng tiền để mua hạt giống, công cụ và các vật phẩm hỗ trợ khác.  

---

## 4. Tính Năng Người Chơi

### 4.1. Tương Tác Môi Trường
- Chuyển đổi linh hoạt giữa không gian trong nhà và ngoài trời.  
- Tương tác với các đối tượng trong thế giới game (cây cối, đồ vật, thú cưng).  

### 4.2. Hệ Thống Inventory
- Quản lý vật phẩm như cây trồng, công cụ, hạt giống.  
- Giao diện người dùng (UI) hiển thị thông tin chi tiết của từng vật phẩm.  

### 4.3. Kỹ Năng Nấu Ăn
- Chế biến nguyên liệu thô thành các món ăn hoàn chỉnh.  
- Mỗi công thức nấu ăn mang lại hiệu ứng khác nhau, tùy thuộc vào nguyên liệu sử dụng.  

---

## 5. Tính Năng AI và Trí Tuệ Nhân Tạo

### 5.1. Dự Báo Thời Tiết (GeminiAPIClient)
- Sử dụng Google Gemini API để tạo dự báo thời tiết chi tiết.  
- Hiển thị thông tin dự báo mỗi khi bắt đầu ngày mới.  

### 5.2. Gợi Ý Công Thức Nấu Ăn
- Phân tích nguyên liệu trong kho và đề xuất các công thức phù hợp.  
- Cung cấp mô tả món ăn và hướng dẫn nấu chi tiết.  

### 5.3. Hệ Thống Thú Cưng Thông Minh
- Thú cưng phản hồi theo hành động của người chơi (vuốt ve, ra lệnh).  
- Có khả năng tự khám phá môi trường và mang về vật phẩm ngẫu nhiên.  

---

## 6. UI và Trải Nghiệm Người Dùng

### 6.1. Bảng Điều Khiển Chính
- Hiển thị các thông tin quan trọng: thời gian, ngày hiện tại, mùa hiện tại.  
- Thanh stamina và số tiền người chơi sở hữu.  

### 6.2. Hệ Thống Menu
- **Inventory**: Xem và quản lý vật phẩm.  
- **Cooking Panel**: Chọn và thực hiện công thức nấu ăn.  
- **Pet Menu**: Tương tác và quản lý thú cưng.  
- **Shop**: Mua bán vật phẩm và công cụ.  

### 6.3. Thông Báo
- Hệ thống message panel hiển thị các thông báo quan trọng.  
- Cung cấp dự báo thời tiết và các thông tin cần thiết khác.  

---

## 7. Hệ Thống Lưu Trữ và Tiến Độ

### 7.1. SaveManager
- Lưu trữ và tải lại tiến độ chơi của người chơi.  
- Bảo tồn trạng thái thế giới game và thông tin nhân vật.  

---

## 8. Cấu Trúc Mã Nguồn

### 8.1. Kiến Trúc Hệ Thống
- Sử dụng **Singleton pattern** cho các manager chính (TimeController, WeatherSystem, v.v.).  
- Thiết kế dựa trên **Component-based design** cho nhân vật và đối tượng trong game.  

### 8.2. Tích Hợp API
- **GeminiAPIClient**: Giao tiếp với Google Gemini API để lấy dữ liệu thời tiết và gợi ý nấu ăn.  
- Xử lý không đồng bộ (sử dụng `async/await`) để đảm bảo hiệu suất khi gọi API.  

### 8.3. Scene Management
- Chuyển đổi giữa các scene: **Main**, **House**, **DayEnd**.  
- **SceneTransitionManager**: Quản lý quá trình chuyển cảnh mượt mà.  

---

## 9. Hướng Phát Triển Tương Lai

### 9.1. Cải Thiện Tiềm Năng
- Thêm các loại cây trồng mới và mở rộng danh sách công thức nấu ăn.  
- Phát triển hệ thống thú cưng với nhiều loài khác nhau.  
- Tích hợp NPC (nhân vật không chơi) và tính năng tương tác xã hội.  

### 9.2. Tối Ưu Hóa
- Cải thiện hiệu suất của các hiệu ứng thời tiết (particle effects).  
- Tối ưu hóa việc gọi API để giảm thời gian chờ.  

### 9.3. Tính Năng Mới
- Hệ thống chăn nuôi gia súc, gia cầm.  
- Khai thác mỏ và thu thập tài nguyên.  
- Chế tạo đồ đạc và trang trí nhà cửa trong game.  

---

## 10. Hướng Dẫn Triển Khai

### 10.1. Cài Đặt Môi Trường
- Sử dụng phiên bản Unity tương thích với mã nguồn game.  
- Cấu hình API keys cho Gemini để kết nối với Google Gemini API.  

### 10.2. Tổng Quan Cấu Trúc Thư Mục
- **`Assets/Scripts`**: Chứa mã nguồn C# của game.  
- **`Assets/Prefabs`**: Lưu trữ các đối tượng prefab đã được định dạng sẵn.  
- **`Assets/Resources`**: Tài nguyên như sprites, âm thanh.  
- **`Assets/Animations`**: Hoạt ảnh cho nhân vật và các đối tượng trong game.  

**Lưu ý**: Tài liệu này được tổng hợp dựa trên mã nguồn và thông tin hiện có. Một số tính năng có thể đang trong giai đoạn phát triển và sẽ được hoàn thiện trong tương lai.