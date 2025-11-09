# SANY Accounting Software - Change Requests & Status Tracker

**Project**: Cascade Accounting System Modifications  
**Client**: School Governing Body (SGB)  
**Last Updated**: November 9, 2025

---

## 📊 **Project Status Overview**

| Category | Total Issues | 🔴 Not Started | 🟡 In Progress | 🟢 Completed | 🔵 Testing |
|----------|--------------|----------------|----------------|---------------|-------------|
| Transactions | 3 | 3 | 0 | 0 | 0 |
| Financial Statements | 1 | 0 | 0 | 1 | 0 |
| User Management | 3 | 2 | 0 | 1 | 0 |
| **TOTAL** | **7** | **5** | **0** | **2** | **0** |

---

## 🎯 **Issue Categories**

### 1. 💰 **Transactions Module** 

#### **Issue #001** - Simplify Account Selection
- **Priority**: 🔴 High
- **Status**: 🔴 Not Started
- **Assignee**: TBD
- **Estimate**: 2-3 days

**Current Behavior**: 
- System shows two accounts (Debit & Credit) for transaction entry
- Manual double-entry accounting interface

**Required Changes**:
- Display only ONE account dropdown
- SGB selects single account from dropdown
- System automatically determines contra account based on account type
- Auto-posting to correct financial statement (Balance Sheet, Income Statement, etc.)

**Technical Impact**:
- Modify `TransactionController.Create()` method
- Update transaction creation views
- Implement automatic account mapping logic

**Example**: Computer purchase → Asset account selected → Auto-posts to Balance Sheet

---

#### **Issue #002** - PDF Invoice Upload Feature
- **Priority**: 🔴 High  
- **Status**: 🔴 Not Started
- **Assignee**: TBD
- **Estimate**: 5-7 days

**Requirements**:
- Add "Upload" button next to Save button in transaction form
- Accept PDF invoice uploads
- OCR/document parsing to extract transaction details
- Automatic posting to correct financial statement

**Technical Components**:
- File upload handling
- PDF parsing/OCR integration
- Document content extraction
- Automated account classification
- Integration with transaction workflow

**Dependencies**: Issue #001 (simplified account selection)

---

#### **Issue #003** - Automated Account Posting Logic
- **Priority**: 🟡 Medium
- **Status**: 🔴 Not Started  
- **Assignee**: TBD
- **Estimate**: 2-3 days

**Requirements**:
- Implement business rules for automatic account determination
- Map account types to financial statement categories
- Handle edge cases and validation

**Account Type Mapping**:
```
Asset Accounts → Balance Sheet
Liability Accounts → Balance Sheet  
Equity Accounts → Balance Sheet
Revenue Accounts → Income Statement
Expense Accounts → Income Statement
```

---

### 2. 📈 **Financial Statements Module**

#### **Issue #004** - Excel Export Functionality
- **Priority**: 🟡 Medium
- **Status**: � Completed
- **Assignee**: AI Assistant
- **Estimate**: 2-3 days
- **Completed**: November 9, 2025

**Requirements**:
- Add "Export to Excel" button on all financial statement reports
- Export current report data to .xlsx format
- Maintain formatting and structure
- Include company information and report parameters

**Reports to Support**:
- Income Statement
- Balance Sheet  
- Cash Flow Statement
- Trial Balance
- General Ledger
- All existing financial reports

**Technical Implementation**:
- ✅ Excel export functionality added to `Bx4Yq1Controller`
- ✅ EPPlus 7.4.0 library integrated for .xlsx generation
- ✅ Export buttons added to all report views
- ✅ Professional formatting with South African Rand currency
- ✅ Six comprehensive export methods implemented
- ✅ Shared ViewModels namespace created for compatibility
- ✅ Service registration configured in dependency injection
- ✅ Build verification completed successfully

**Files Modified**:
- `Cascade.csproj`, `Services/ExcelExportService.cs`, `Kz9Xm4Flux.cs`
- `Zr9Kq6/Bx4Yq1Controller.cs`, `ViewModels/FinancialReportViewModels.cs`
- `Views/_ViewImports.cshtml` and 6 financial report views

---

### 3. 👥 **User Management & Roles**

#### **Issue #005** - Rename Accountant Role
- **Priority**: 🟢 Low
- **Status**: � Completed
- **Assignee**: AI Assistant  
- **Estimate**: 1 day
- **Completed**: November 9, 2025

**Required Changes**:
- ✅ Rename "Accountant" role to "SGB Treasurer" 
- ✅ Update all UI references
- ✅ Maintain existing permissions and functionality
- ✅ Update role descriptions and labels

**Technical Implementation**:
- ✅ Updated role enum `Bz5Xw6Permission.Accountant` → `Bz5Xw6Permission.SGBTreasurer`
- ✅ Modified database seeding to create "SGB Treasurer" role instead of "Accountant"
- ✅ **CRITICAL**: Added migration logic to handle existing users safely
- ✅ Updated default user from "accountant" → "treasurer" with appropriate email
- ✅ Changed all authorization attributes from `[Authorize(Roles = "Accountant")]` to `[Authorize(Roles = "SGB Treasurer")]`
- ✅ Updated JavaScript variables from `isAccountant` → `isTreasurer`
- ✅ Modified controller routing logic to use new role name
- ✅ Verified build completion with no compilation errors

**Migration Safety Features**:
- ✅ **Existing User Protection**: All existing "Accountant" users automatically migrated to "SGB Treasurer" role
- ✅ **Role Property Update**: User.Role enum property updated from Accountant to SGBTreasurer
- ✅ **Backward Compatibility**: Existing "accountant" username preserved and migrated
- ✅ **Database Cleanup**: Old "Accountant" role removed after successful migration
- ✅ **Zero Downtime**: Users maintain access during role transition

**Files Modified**:
- `Fx9Kl2/Aq3Zh4Service.cs` (Role enum)
- `A1B2C3D4/SxVb2DbInjector.cs` (Database seeding)
- `Zr9Kq6/Bx4Yq1Controller.cs` (Financial reports controller)
- `Zr9Kq6/Wy5Rt2Controller.cs` (Home routing)
- `Zr9Kq6/Qx8Np3Controller.cs` (Authentication routing)
- `Areas/Accountant/Controllers/DashboardController.cs`
- `Areas/Accountant/Controllers/TransactionController.cs`
- `wwwroot/js/notes-modal.js` (Frontend JavaScript)
- `issues.md` (Documentation update)

---

#### **Issue #006** - Remove SuperAdmin Role  
- **Priority**: 🟢 Low
- **Status**: 🔴 Not Started
- **Assignee**: TBD
- **Estimate**: 1-2 days

**Requirements**:
- Remove SuperAdmin role entirely from system
- Clean up database references
- Remove SuperAdmin-specific UI elements
- Ensure no broken functionality

**Impact Areas**:
- Authentication and authorization
- Role-based routing  
- Database cleanup
- UI navigation updates

---

#### **Issue #007** - Admin Role Clarification
- **Priority**: 🟢 Low  
- **Status**: 🔴 Not Started
- **Assignee**: TBD
- **Estimate**: 0.5 days

**Requirements**:
- Keep Admin account unchanged
- Ensure Admin can add SGB Treasurer accounts
- Verify Admin can review all financial statements
- Document Admin capabilities clearly

---

## 📋 **Implementation Roadmap**

### **Phase 1: Core Transaction Changes** (Week 1-2)
1. Issue #001 - Simplify Account Selection
2. Issue #003 - Automated Account Posting Logic

### **Phase 2: Advanced Features** (Week 3-4)  
1. Issue #002 - PDF Invoice Upload Feature
2. Issue #004 - Excel Export Functionality

### **Phase 3: User Management Updates** (Week 5)
1. Issue #005 - Rename Accountant Role
2. Issue #006 - Remove SuperAdmin Role  
3. Issue #007 - Admin Role Clarification

---

## 🔧 **Technical Notes**

### **Current System Architecture**:
- **Framework**: ASP.NET Core 8.0
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity
- **Roles**: SuperAdmin, Admin, SGB Treasurer

### **Key Files for Modifications**:
- `Areas/Accountant/Controllers/TransactionController.cs`
- `Zr9Kq6/Bx4Yq1Controller.cs` (Reports)
- `Fx9Kl2/Aq3Zh4Service.cs` (User roles)
- Transaction views and forms
- Financial statement report views

### **Dependencies & Considerations**:
- PDF processing library integration
- Excel export library (EPPlus)
- Account type classification logic
- Database migration for role changes
- User training and documentation updates

---

## 📝 **Change Log**

| Date | Issue | Action | Notes |
|------|-------|---------|-------|
| 2025-11-09 | All | Initial documentation | Structured requirements from client request |

---

**Next Review Date**: TBD  
**Project Manager**: TBD  
**Technical Lead**: TBD