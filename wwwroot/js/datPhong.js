// DatPhong Management JavaScript
window.DatPhongApp = {
    phongIndex: 0,
    dichVuIndex: 0,
    currentAction: null,
    currentId: null,

    // Initialize index page
    initIndexPage() {
        this.loadDatPhongList();
        this.setupEventListeners();
    },

    // Initialize create form
    initCreateForm() {
        this.loadSelectOptions();
        this.setupCreateForm();
        this.setupDynamicRows();
        this.setupCalculation();
    },

    // Initialize details page
    initDetailsPage(id) {
        this.loadDatPhongDetails(id);
        this.setupDetailsPage();
    },

    // Load dat phong list
    async loadDatPhongList() {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('dataContainer').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'none';

            const data = await HotelApp.apiFetch('/DatPhong');
            
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('dataContainer').style.display = 'block';
            
            this.renderDatPhongTable(data);
        } catch (error) {
            console.error('Error loading dat phong list:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải danh sách đặt phòng: ' + error.message);
        }
    },

    // Render dat phong table
    renderDatPhongTable(data) {
        const tbody = document.getElementById('datPhongTableBody');
        tbody.innerHTML = '';

        if (!data || data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="9" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2"></i><br>
                        Không có dữ liệu
                    </td>
                </tr>
            `;
            return;
        }

        data.forEach(item => {
            const row = document.createElement('tr');
            const statusBadge = this.getStatusBadge(item.trangThai);
            const actionButtons = this.getActionButtons(item.maDatPhong, item.trangThai);
            
            row.innerHTML = `
                <td>${item.maDatPhong || ''}</td>
                <td>${item.khachHang?.hoTen || ''}</td>
                <td>${item.nhanVien?.hoTen || ''}</td>
                <td>${HotelApp.formatDate(item.ngayDat)}</td>
                <td>${HotelApp.formatDate(item.ngayNhan)}</td>
                <td>${HotelApp.formatDate(item.ngayTra)}</td>
                <td>${HotelApp.formatCurrency(item.tongTien || 0)}</td>
                <td>${statusBadge}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <a href="/DatPhong/Details/${item.maDatPhong}" class="btn btn-outline-info" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </a>
                        ${actionButtons}
                    </div>
                </td>
            `;
            tbody.appendChild(row);
        });
    },

    // Get status badge
    getStatusBadge(status) {
        const statusMap = {
            'DaDat': '<span class="badge bg-warning">Đã đặt</span>',
            'DaNhan': '<span class="badge bg-info">Đã nhận</span>',
            'DaTra': '<span class="badge bg-success">Đã trả</span>',
            'Huy': '<span class="badge bg-danger">Hủy</span>'
        };
        return statusMap[status] || '<span class="badge bg-secondary">' + status + '</span>';
    },

    // Get action buttons
    getActionButtons(id, status) {
        let buttons = '';
        
        if (status === 'DaDat') {
            buttons += `<button type="button" class="btn btn-outline-success" onclick="DatPhongApp.checkin(${id})" title="Check-in">
                <i class="fas fa-sign-in-alt"></i>
            </button>`;
        }
        
        if (status === 'DaNhan') {
            buttons += `<button type="button" class="btn btn-outline-warning" onclick="DatPhongApp.checkout(${id})" title="Check-out">
                <i class="fas fa-sign-out-alt"></i>
            </button>`;
        }
        
        return buttons;
    },

    // Setup event listeners
    setupEventListeners() {
        const actionModal = document.getElementById('actionModal');
        if (actionModal) {
            const confirmActionBtn = document.getElementById('confirmActionBtn');
            if (confirmActionBtn) {
                confirmActionBtn.addEventListener('click', () => {
                    this.confirmAction();
                });
            }
        }
    },

    // Load select options for create form
    async loadSelectOptions() {
        try {
            // Load khach hang
            const khachHangData = await HotelApp.apiFetch('/KhachHang');
            const khachHangSelect = document.getElementById('khachHangId');
            khachHangData.forEach(item => {
                const option = document.createElement('option');
                option.value = item.maDatPhong;
                option.textContent = item.tenKhachHang;
                khachHangSelect.appendChild(option);
            });

            // Load nhan vien
            const nhanVienData = await HotelApp.apiFetch('/NhanVien');
            const nhanVienSelect = document.getElementById('nhanVienId');
            nhanVienData.forEach(item => {
                const option = document.createElement('option');
                option.value = item.maDatPhong;
                option.textContent = item.tenNhanVien;
                nhanVienSelect.appendChild(option);
            });

            // Load phong (trống)
            const phongData = await HotelApp.apiFetch('/Phong');
            const phongSelects = document.querySelectorAll('.phong-select');
            phongSelects.forEach(select => {
                phongData.filter(p => p.tinhTrang === 'Trong').forEach(item => {
                    const option = document.createElement('option');
                    option.value = item.maDatPhong;
                    option.textContent = `${item.SoPhong} - ${item.loaiPhong?.tenLoaiPhong || ''}`;
                    option.dataset.gia = item.loaiPhong?.giaCoBan || 0;
                    select.appendChild(option);
                });
            });

            // Load dich vu
            const dichVuData = await HotelApp.apiFetch('/DichVu');
            const dichVuSelects = document.querySelectorAll('.dichvu-select');
            dichVuSelects.forEach(select => {
                dichVuData.forEach(item => {
                    const option = document.createElement('option');
                    option.value = item.maDatPhong;
                    option.textContent = item.tenDichVu;
                    option.dataset.gia = item.gia || 0;
                    select.appendChild(option);
                });
            });

        } catch (error) {
            console.error('Error loading select options:', error);
            HotelApp.showError('Không thể tải dữ liệu cho form: ' + error.message);
        }
    },

    // Setup create form
    setupCreateForm() {
        const form = document.getElementById('createForm');
        const submitBtn = document.getElementById('submitBtn');

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            if (!HotelApp.validateForm('createForm')) {
                return;
            }

            try {
                HotelApp.showLoading(submitBtn);
                
                const formData = new FormData(form);
                const data = this.collectFormData(formData);
                
                await HotelApp.apiFetch('/DatPhong', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });

                HotelApp.showSuccess('Đặt phòng thành công!');
                window.location.href = '/DatPhong';
            } catch (error) {
                console.error('Error creating dat phong:', error);
                HotelApp.showError('Không thể đặt phòng: ' + error.message);
            } finally {
                HotelApp.hideLoading(submitBtn, 'Đặt phòng');
            }
        });
    },

    // Collect form data
    collectFormData(formData) {
        const data = {
            khachHangId: parseInt(formData.get('khachHangId')),
            nhanVienId: parseInt(formData.get('nhanVienId')),
            ngayNhan: formData.get('ngayNhan'),
            ngayTra: formData.get('ngayTra'),
            chiTietDatPhongs: [],
            suDungDichVus: []
        };

        // Collect phong data
        const phongRows = document.querySelectorAll('#phongContainer .dynamic-row');
        phongRows.forEach((row, index) => {
            const phongId = row.querySelector('.phong-select').value;
            const donGia = parseFloat(row.querySelector('.don-gia').value);
            const soDem = parseInt(row.querySelector('.so-dem').value);
            
            if (phongId && donGia && soDem) {
                data.chiTietDatPhongs.push({
                    phongId: parseInt(phongId),
                    donGia: donGia,
                    soDem: soDem
                });
            }
        });

        // Collect dich vu data
        const dichVuRows = document.querySelectorAll('#dichVuContainer .dynamic-row');
        dichVuRows.forEach((row, index) => {
            const dichVuId = row.querySelector('.dichvu-select').value;
            const soLuong = parseInt(row.querySelector('.so-luong').value);
            const donGia = parseFloat(row.querySelector('.don-gia-dichvu').value);
            
            if (dichVuId && soLuong && donGia) {
                data.suDungDichVus.push({
                    dichVuId: parseInt(dichVuId),
                    soLuong: soLuong,
                    donGia: donGia
                });
            }
        });

        return data;
    },

    // Setup dynamic rows
    setupDynamicRows() {
        // Add phong row
        document.getElementById('addPhongBtn').addEventListener('click', () => {
            this.addPhongRow();
        });

        // Add dich vu row
        document.getElementById('addDichVuBtn').addEventListener('click', () => {
            this.addDichVuRow();
        });
    },

    // Add phong row
    addPhongRow() {
        this.phongIndex++;
        const container = document.getElementById('phongContainer');
        const newRow = document.createElement('div');
        newRow.className = 'dynamic-row';
        newRow.dataset.index = this.phongIndex;
        
        newRow.innerHTML = `
            <div class="row">
                <div class="col-md-4">
                    <div class="mb-3">
                        <label class="form-label">Phòng <span class="text-danger">*</span></label>
                        <select class="form-select phong-select" name="chiTietDatPhongs[${this.phongIndex}].phongId" required>
                            <option value="">Chọn phòng...</option>
                        </select>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="mb-3">
                        <label class="form-label">Đơn giá (VNĐ) <span class="text-danger">*</span></label>
                        <input type="number" class="form-control don-gia" name="chiTietDatPhongs[${this.phongIndex}].donGia" min="0" step="1000" required>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="mb-3">
                        <label class="form-label">Số đêm <span class="text-danger">*</span></label>
                        <input type="number" class="form-control so-dem" name="chiTietDatPhongs[${this.phongIndex}].soDem" min="1" required>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="mb-3">
                        <label class="form-label">&nbsp;</label>
                        <button type="button" class="btn btn-danger btn-sm w-100 remove-phong">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        container.appendChild(newRow);
        this.loadPhongOptions(newRow.querySelector('.phong-select'));
        this.setupRemoveButton(newRow, '.remove-phong');
        this.setupCalculation();
    },

    // Add dich vu row
    addDichVuRow() {
        this.dichVuIndex++;
        const container = document.getElementById('dichVuContainer');
        const newRow = document.createElement('div');
        newRow.className = 'dynamic-row';
        newRow.dataset.index = this.dichVuIndex;
        
        newRow.innerHTML = `
            <div class="row">
                <div class="col-md-4">
                    <div class="mb-3">
                        <label class="form-label">Dịch vụ</label>
                        <select class="form-select dichvu-select" name="suDungDichVus[${this.dichVuIndex}].dichVuId">
                            <option value="">Chọn dịch vụ...</option>
                        </select>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="mb-3">
                        <label class="form-label">Số lượng</label>
                        <input type="number" class="form-control so-luong" name="suDungDichVus[${this.dichVuIndex}].soLuong" min="1" value="1">
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="mb-3">
                        <label class="form-label">Đơn giá (VNĐ)</label>
                        <input type="number" class="form-control don-gia-dichvu" name="suDungDichVus[${this.dichVuIndex}].donGia" min="0" step="1000">
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="mb-3">
                        <label class="form-label">&nbsp;</label>
                        <button type="button" class="btn btn-danger btn-sm w-100 remove-dichvu">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        container.appendChild(newRow);
        this.loadDichVuOptions(newRow.querySelector('.dichvu-select'));
        this.setupRemoveButton(newRow, '.remove-dichvu');
        this.setupCalculation();
    },

    // Load phong options
    async loadPhongOptions(select) {
        try {
            const phongData = await HotelApp.apiFetch('/Phong');
            phongData.filter(p => p.tinhTrang === 'Trong').forEach(item => {
                const option = document.createElement('option');
                option.value = item.maDatPhong;
                option.textContent = `${item.soPhong} - ${item.loaiPhong?.tenLoaiPhong || ''}`;
                option.dataset.gia = item.loaiPhong?.giaCoBan || 0;
                select.appendChild(option);
            });
        } catch (error) {
            console.error('Error loading phong options:', error);
        }
    },

    // Load dich vu options
    async loadDichVuOptions(select) {
        try {
            const dichVuData = await HotelApp.apiFetch('/DichVu');
            dichVuData.forEach(item => {
                const option = document.createElement('option');
                option.value = item.maDatPhong;
                option.textContent = item.tenDichVu;
                option.dataset.gia = item.gia || 0;
                select.appendChild(option);
            });
        } catch (error) {
            console.error('Error loading dich vu options:', error);
        }
    },

    // Setup remove button
    setupRemoveButton(row, selector) {
        const removeBtn = row.querySelector(selector);
        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                row.remove();
                this.calculateTotal();
            });
        }
    },

    // Setup calculation
    setupCalculation() {
        // Phong calculation
        document.addEventListener('change', (e) => {
            if (e.target.classList.contains('phong-select')) {
                const option = e.target.selectedOptions[0];
                if (option && option.dataset.gia) {
                    const donGiaInput = e.target.closest('.dynamic-row').querySelector('.don-gia');
                    donGiaInput.value = option.dataset.gia;
                    this.calculateTotal();
                }
            }
            
            if (e.target.classList.contains('dichvu-select')) {
                const option = e.target.selectedOptions[0];
                if (option && option.dataset.gia) {
                    const donGiaInput = e.target.closest('.dynamic-row').querySelector('.don-gia-dichvu');
                    donGiaInput.value = option.dataset.gia;
                    this.calculateTotal();
                }
            }
            
            if (e.target.classList.contains('don-gia') || 
                e.target.classList.contains('so-dem') || 
                e.target.classList.contains('so-luong') || 
                e.target.classList.contains('don-gia-dichvu')) {
                this.calculateTotal();
            }
        });
    },

    // Calculate total
    calculateTotal() {
        let tongTienPhong = 0;
        let tongTienDichVu = 0;

        // Calculate phong total
        const phongRows = document.querySelectorAll('#phongContainer .dynamic-row');
        phongRows.forEach(row => {
            const donGia = parseFloat(row.querySelector('.don-gia').value) || 0;
            const soDem = parseInt(row.querySelector('.so-dem').value) || 0;
            tongTienPhong += donGia * soDem;
        });

        // Calculate dich vu total
        const dichVuRows = document.querySelectorAll('#dichVuContainer .dynamic-row');
        dichVuRows.forEach(row => {
            const donGia = parseFloat(row.querySelector('.don-gia-dichvu').value) || 0;
            const soLuong = parseInt(row.querySelector('.so-luong').value) || 0;
            tongTienDichVu += donGia * soLuong;
        });

        // Update display
        document.getElementById('tongTienPhong').textContent = HotelApp.formatCurrency(tongTienPhong);
        document.getElementById('tongTienDichVu').textContent = HotelApp.formatCurrency(tongTienDichVu);
        document.getElementById('tongCong').textContent = HotelApp.formatCurrency(tongTienPhong + tongTienDichVu);
    },

    // Load dat phong details
    async loadDatPhongDetails(id) {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('errorMessage').style.display = 'none';
            document.getElementById('detailsContent').style.display = 'none';

            const data = await HotelApp.apiFetch(`/DatPhong/${id}`);
            
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('detailsContent').style.display = 'block';
            
            this.renderDatPhongDetails(data);
        } catch (error) {
            console.error('Error loading dat phong details:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải thông tin đặt phòng: ' + error.message);
        }
    },

    // Render dat phong details
    renderDatPhongDetails(data) {
        // Basic info
        document.getElementById('id').textContent = data.maDatPhong || '';
        document.getElementById('khachHang').textContent = data.khachHang?.hoTen || '';
        document.getElementById('nhanVien').textContent = data.nhanVien?.hoTen || '';
        document.getElementById('ngayDat').textContent = HotelApp.formatDate(data.ngayDat);
        document.getElementById('ngayNhan').textContent = HotelApp.formatDate(data.ngayNhan);
        document.getElementById('ngayTra').textContent = HotelApp.formatDate(data.ngayTra);
        document.getElementById('trangThai').innerHTML = this.getStatusBadge(data.trangThai);

        // Chi tiet phong
        this.renderChiTietPhong(data.chiTietDatPhongs || []);
        
        // Dich vu
        this.renderDichVu(data.suDungDichVus || []);

        // Total
        this.renderTotal(data);

        // Action buttons
        this.setupActionButtons(data.maDatPhong, data.trangThai);
    },

    // Render chi tiet phong
    renderChiTietPhong(chiTietPhongs) {
        const tbody = document.getElementById('chiTietPhongTableBody');
        tbody.innerHTML = '';

        if (chiTietPhongs.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>';
            return;
        }

        chiTietPhongs.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.phong?.SoPhong || ''}</td>
                <td>${HotelApp.formatCurrency(item.donGia || 0)}</td>
                <td>${item.soDem || ''}</td>
                <td>${HotelApp.formatCurrency((item.donGia || 0) * (item.soDem || 0))}</td>
            `;
            tbody.appendChild(row);
        });
    },

    // Render dich vu
    renderDichVu(dichVus) {
        const tbody = document.getElementById('dichVuTableBody');
        tbody.innerHTML = '';

        if (dichVus.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>';
            return;
        }

        dichVus.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.dichVu?.tenDichVu || ''}</td>
                <td>${item.soLuong || ''}</td>
                <td>${HotelApp.formatCurrency(item.donGia || 0)}</td>
                <td>${HotelApp.formatCurrency((item.donGia || 0) * (item.soLuong || 0))}</td>
            `;
            tbody.appendChild(row);
        });
    },

    // Render total
    renderTotal(data) {
        const tongTienPhong = (data.chiTietDatPhongs || []).reduce((sum, item) => 
            sum + ((item.donGia || 0) * (item.soDem || 0)), 0);
        const tongTienDichVu = (data.suDungDichVus || []).reduce((sum, item) => 
            sum + ((item.donGia || 0) * (item.soLuong || 0)), 0);

        document.getElementById('tongTienPhong').textContent = HotelApp.formatCurrency(tongTienPhong);
        document.getElementById('tongTienDichVu').textContent = HotelApp.formatCurrency(tongTienDichVu);
        document.getElementById('tongCong').textContent = HotelApp.formatCurrency(tongTienPhong + tongTienDichVu);
    },

    // Setup action buttons
    setupActionButtons(id, status) {
        const checkinBtn = document.getElementById('checkinBtn');
        const checkoutBtn = document.getElementById('checkoutBtn');
        const cancelBtn = document.getElementById('cancelBtn');

        if (checkinBtn) checkinBtn.style.display = status === 'DaDat' ? 'inline-block' : 'none';
        if (checkoutBtn) checkoutBtn.style.display = status === 'DaNhan' ? 'inline-block' : 'none';
        if (cancelBtn) cancelBtn.style.display = status === 'DaDat' ? 'inline-block' : 'none';

        if (checkinBtn) {
            checkinBtn.onclick = () => this.checkin(id);
        }
        if (checkoutBtn) {
            checkoutBtn.onclick = () => this.checkout(id);
        }
        if (cancelBtn) {
            cancelBtn.onclick = () => this.cancel(id);
        }
    },

    // Setup details page
    setupDetailsPage() {
        // Action modal setup is already done in setupEventListeners
    },

    // Check-in
    async checkin(id) {
        this.currentAction = 'checkin';
        this.currentId = id;
        this.showActionModal('Check-in', 'Bạn có chắc chắn muốn check-in cho đặt phòng này không?');
    },

    // Check-out
    async checkout(id) {
        this.currentAction = 'checkout';
        this.currentId = id;
        this.showActionModal('Check-out', 'Bạn có chắc chắn muốn check-out cho đặt phòng này không?');
    },

    // Cancel
    async cancel(id) {
        this.currentAction = 'cancel';
        this.currentId = id;
        this.showActionModal('Hủy đặt phòng', 'Bạn có chắc chắn muốn hủy đặt phòng này không?');
    },

    // Show action modal
    showActionModal(title, message) {
        document.getElementById('actionModalTitle').textContent = title;
        document.getElementById('actionModalMessage').textContent = message;
        const modal = new bootstrap.Modal(document.getElementById('actionModal'));
        modal.show();
    },

    // Confirm action
    async confirmAction() {
        try {
            let endpoint = '';
            let method = 'PUT';
            
            switch (this.currentAction) {
                case 'checkin':
                    endpoint = `/DatPhong/${this.currentId}/checkin`;
                    break;
                case 'checkout':
                    endpoint = `/DatPhong/${this.currentId}/checkout`;
                    break;
                case 'cancel':
                    endpoint = `/DatPhong/${this.currentId}`;
                    method = 'DELETE';
                    break;
            }

            await HotelApp.apiFetch(endpoint, { method });
            
            const actionNames = {
                'checkin': 'Check-in',
                'checkout': 'Check-out',
                'cancel': 'Hủy đặt phòng'
            };
            
            HotelApp.showSuccess(`${actionNames[this.currentAction]} thành công!`);
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('actionModal'));
            modal.hide();
            
            // Reload page or redirect
            if (window.location.pathname.includes('/Details/')) {
                this.loadDatPhongDetails(this.currentId);
            } else {
                this.loadDatPhongList();
            }
        } catch (error) {
            console.error('Error performing action:', error);
            HotelApp.showError('Không thể thực hiện thao tác: ' + error.message);
        }
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Auto-initialize based on current page
    const path = window.location.pathname;
    
    if (path.includes('/DatPhong/Index') || path === '/DatPhong' || path === '/DatPhong/') {
        DatPhongApp.initIndexPage();
    }
});
