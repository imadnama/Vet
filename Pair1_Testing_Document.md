# ClinicVets — Pair 1 Testing & Quality Document
**Pair:** Muhammad Bdei'i & Zein Aldin Asadi  
**Module:** Login, Employee Registration, Customer Management  
**Date:** 25/5/2026

---

## 1. CFG — Control Flow Graphs

### Function 1: `ValidateUsername(string username, out string error)`
**File:** `src/ClinicVets.g1/Validation/EmployeeValidator.cs`

```
N1 [START]
  │
  ▼
N2: IsNullOrWhiteSpace(username)?
  │ YES → N3: error = "Username is required."  → N10 [return false]
  │ NO
  ▼
N4: username.Length < 6  OR  username.Length > 8?
  │ YES → N5: error = "Username must be 6–8 characters." → N10 [return false]
  │ NO
  ▼
N6: [LOOP] foreach char c in username
  │
  ▼
N7: char.IsDigit(c)?
  │ YES → N8: digitCount++  ──────────────────────────┐
  │ NO                                                  │
  ▼                                                     │
N9: !char.IsAsciiLetter(c)?                            │
  │ YES → N9a: error = "English letters/digits only" → N10 [return false]
  │ NO                                                  │
  └──────────────────────────────────────────────────►─┘
                                                        │
                                              [LOOP BACK to N6 or EXIT]
                                                        │
                                                       ▼
N11: digitCount > 2?
  │ YES → N12: error = "At most 2 digits." → N10 [return false]
  │ NO
  ▼
N13 [return true]
```

**Nodes:** N1, N2, N3, N4, N5, N6, N7, N8, N9, N9a, N10, N11, N12, N13  
**Edges:** 14  
**Cyclomatic Complexity:** V(G) = E – N + 2 = 14 – 13 + 2 = **3**  
**Independent paths:**
1. N1→N2→N3→N10 (empty username)
2. N1→N2→N4→N5→N10 (wrong length)
3. N1→N2→N4→N6→N7→N8→N11→N13 (valid username)
4. N1→N2→N4→N6→N9→N9a→N10 (non-English char)
5. N1→N2→N4→N6→N7→N8→N11→N12→N10 (>2 digits)

---

### Function 2: `ValidatePassword(string password, out string error)`
**File:** `src/ClinicVets.g1/Validation/EmployeeValidator.cs`

```
N1 [START]
  │
  ▼
N2: IsNullOrWhiteSpace(password)?
  │ YES → N3: error = "Password is required." → N14 [return false]
  │ NO
  ▼
N4: password.Length < 8  OR  > 10?
  │ YES → N5: error = "Password must be 8–10 characters." → N14 [return false]
  │ NO
  ▼
N6: [LOOP] foreach char c in password
  │
  ▼
N7: IsLetter(c)?
  │ YES → N8: hasLetter = true ──────────────────────────┐
  │ NO                                                     │
  ▼                                                        │
N9: IsDigit(c)?                                           │
  │ YES → N10: hasDigit = true ──────────────────────────►┤
  │ NO                                                     │
  ▼                                                        │
N11: c in allowedSpecials { !, #, $, , }?                  │
  │ YES → N12: hasSpecial = true ────────────────────────►┤
  │ NO ─────────────────────────────────────────────────►─┘
                                                           │
                                              [LOOP BACK to N6 or EXIT]
                                                           │
                                                          ▼
N13: !hasLetter?
  │ YES → N14a: error = "At least one letter." → N14 [return false]
  │ NO
  ▼
N15: !hasDigit?
  │ YES → N16: error = "At least one digit." → N14 [return false]
  │ NO
  ▼
N17: !hasSpecial?
  │ YES → N18: error = "At least one special char." → N14 [return false]
  │ NO
  ▼
N19 [return true]
```

**Nodes:** 19  **Edges:** 21  
**Cyclomatic Complexity:** V(G) = 21 – 19 + 2 = **4**  
**Independent paths:**
1. Empty → N14 (null check)
2. Wrong length → N14
3. Missing letter → N14
4. Missing digit → N14
5. Missing special char → N14
6. All conditions met → N19 (true)

---

## 2. User Stories

### US-1: Employee Login
> **As a** clinic staff member,  
> **I want to** log in with my username and password,  
> **So that** I can access the system features according to my role (Veterinarian or Secretary).

**Acceptance criteria:**
- Entering a correct username + password opens the dashboard.
- Entering wrong credentials shows an error message.
- A Veterinarian sees Visit & Treatment buttons; a Secretary sees Customer Management buttons.

---

### US-2: Register New Employee
> **As a** clinic manager,  
> **I want to** register a new employee with a username, password, employee number, email, national ID, and role,  
> **So that** the new employee can log in and use the system.

**Acceptance criteria:**
- Username must be 6–8 characters, at most 2 digits.
- Password must be 8–10 characters with at least one letter, digit, and special character (! # $ ,).
- Employee number must be exactly 4 digits.
- Duplicate username or employee number is rejected with an error message.
- On success, a confirmation message is displayed and the form closes.

---

### US-3: Register and Search Customer
> **As a** secretary,  
> **I want to** register a new customer (animal owner) and later search for them by ID or phone,  
> **So that** I can view their details and the list of animals they own.

**Acceptance criteria:**
- Full name: letters only. National ID: exactly 9 digits. Phone: Israeli format (05X-XXXXXXX).
- Duplicate national ID is rejected.
- Search by ID or phone returns the customer's details and all linked animals in a table.
- This feature is hidden from Veterinarian users.

---

## 3. Test Cases (from 2 User Stories)

### From US-2 (Employee Registration)

| # | Test Case ID | Description | Input | Expected Output | Pass/Fail |
|---|---|---|---|---|---|
| 1 | TC-US2-01 | Valid employee registration | Username: moh123, Password: Pass12!#, EmpNum: 1234, Email: m@c.com, ID: 123456789, Role: Vet | "Employee registered successfully!" message, form closes | Pass |
| 2 | TC-US2-02 | Invalid username — too short | Username: ab1 | Error: "Username must be 6–8 characters long." | Pass |
| 3 | TC-US2-03 | Invalid password — no special char | Password: Password1 | Error: "Password must contain at least one special character" | Pass |
| 4 | TC-US2-04 | Duplicate employee number | EmpNum: 1234 (already exists) | Error: "Registration failed. Username or Employee Number already exists." | Pass |

### From US-3 (Customer Management)

| # | Test Case ID | Description | Input | Expected Output | Pass/Fail |
|---|---|---|---|---|---|
| 5 | TC-US3-01 | Valid customer registration | Name: Ahmad Salem, ID: 987654321, Phone: 050-1234567, Email: a@b.com | "Customer registered successfully!" | Pass |
| 6 | TC-US3-02 | Invalid customer ID | ID: 12345 (5 digits) | Error: "Customer ID must be exactly 9 digits." | Pass |
| 7 | TC-US3-03 | Search customer by valid ID | Search: 987654321 | Customer details displayed + linked animals shown | Pass |
| 8 | TC-US3-04 | Search customer — not found | Search: 000000000 | Status: "No customer found with the given ID." | Pass |

---

## 4. Functional Tests and GUI Tests

### Functional Tests

**FT-1: Login Authentication**
- **Description:** Verify that `AuthService.Login()` returns `true` and sets the employee when valid credentials are provided, and returns `false` for wrong password.
- **Steps:**
  1. Register employee with username `moh123` and password `Pass12!#`.
  2. Call `Login("moh123", "Pass12!#", out employee)`.
  3. Assert result is `true` and `employee.Username == "moh123"`.
  4. Call `Login("moh123", "WrongPass1!", out employee)`.
  5. Assert result is `false` and `employee == null`.
- **Expected:** Step 3 passes, step 5 passes. ✅

**FT-2: Customer Registration Validation**
- **Description:** Verify that `CustomerService.RegisterCustomer()` rejects invalid data and accepts valid data.
- **Steps:**
  1. Call `RegisterCustomer` with ID = "12345" (too short).
  2. Assert `error == "Customer ID must be exactly 9 digits."`.
  3. Call with all valid fields.
  4. Assert returns `true` and customer is found in `GetByNationalId()`.
- **Expected:** Step 2 and step 4 pass. ✅

---

### GUI Tests

**GUI-1: Error label shown in red on failed login**
- **Steps:**
  1. Run the application.
  2. Enter username: `wronguser`, password: `wrongpass`.
  3. Click SIGN IN.
- **Expected:** A red error message "Invalid username or password. Please try again." appears below the password field. Password field is cleared.
- **Result:** ✅ Error label visible, red color, password cleared.

**GUI-2: Customer form clears after successful registration**
- **Steps:**
  1. Log in as Secretary.
  2. Open "Register Customer".
  3. Fill all valid fields and click SAVE CUSTOMER.
  4. Observe the form after the success message is dismissed.
- **Expected:** All text fields become empty and focus returns to the Full Name field.
- **Result:** ✅ Fields cleared, focus on Full Name.

---

## 5. Test Scenario, Test Cases, and Test Scripts

### Scenario: Employee Registration Validation
**Goal:** Verify that the registration form enforces all field rules before saving to the database.

| Step | Action | Expected |
|------|--------|----------|
| 1 | Open application | Splash screen shown, then login form |
| 2 | Click "Register New Employee" | RegisterEmployeeForm opens |
| 3 | Submit empty form | Error: "Full name is required." |
| 4 | Enter valid full name, invalid username "ab" | Error: "Username must be 6–8 characters long." |
| 5 | Enter valid username "moh123", password "abc" (no digit/special) | Error: "Password must be 8–10 characters long." |
| 6 | Enter valid password "Pass12!#", employee number "12" | Error: "Employee number must be exactly 4 digits." |
| 7 | Enter all valid fields | Success message, form closes |

---

**Test Case S-TC-01: Valid Full Registration**

| Field | Value |
|-------|-------|
| Full Name | Muhammad Bdei |
| Username | moh123 |
| Password | Pass12!# |
| Confirm Password | Pass12!# |
| Employee Number | 1234 |
| Email | moh@clinic.com |
| National ID | 123456789 |
| Role | Veterinarian |

- **Expected:** MessageBox "Employee 'moh123' registered successfully!" appears. Form closes.
- **Test script:** Fill fields above → click REGISTER EMPLOYEE → verify dialog → close → attempt login with same credentials → verify dashboard opens.

---

**Test Case S-TC-02: Registration with Duplicate Username**

| Field | Value |
|-------|-------|
| Username | moh123 (already registered in S-TC-01) |
| All other fields | Different valid values |

- **Expected:** Error label shows "Registration failed. Username or Employee Number already exists."
- **Test script:** Ensure S-TC-01 ran first → open register form → enter username "moh123" → click REGISTER → verify error shown, form does NOT close.

---

## 6. Boundary Value Table

| Variable | Min-1 | Min | Min+1 | Nominal | Max-1 | Max | Max+1 | Type |
|---|---|---|---|---|---|---|---|---|
| **Username length** | 5 chars | 6 chars ✅ | 7 chars ✅ | 7 chars ✅ | 7 chars ✅ | 8 chars ✅ | 9 chars ❌ | Integer |
| **Username digits** | — | 0 digits ✅ | 1 digit ✅ | 1 digit ✅ | 2 digits ✅ | 2 digits ✅ | 3 digits ❌ | Integer |
| **Password length** | 7 chars ❌ | 8 chars ✅ | 9 chars ✅ | 9 chars ✅ | 9 chars ✅ | 10 chars ✅ | 11 chars ❌ | Integer |
| **Employee number** | 3 digits ❌ | 4 digits ✅ | — | 4 digits ✅ | — | 4 digits ✅ | 5 digits ❌ | Exact |
| **Customer ID** | 8 digits ❌ | 9 digits ✅ | — | 9 digits ✅ | — | 9 digits ✅ | 10 digits ❌ | Exact |
| **Animal weight** | 0.0 kg ❌ | 0.1 kg ✅ | 0.2 kg ✅ | 10 kg ✅ | 99.9 kg ✅ | 100 kg ✅ | 100.1 kg ❌ | Decimal |

**Concrete boundary test values:**

| Test | Username | Password | EmpNum | Customer ID | Expected |
|---|---|---|---|---|---|
| Below min length | "ab1" | — | — | — | INVALID |
| At min length | "abc1de" | — | — | — | VALID |
| At max length | "abc12def"* | — | — | — | VALID |
| Above max length | "abc123def" | — | — | — | INVALID |
| Password too short | — | "Pass1!" | — | — | INVALID |
| Password at min | — | "Pass12!#" | — | — | VALID |
| Password at max | — | "Pass123!#" | — | — | VALID |
| EmpNum 3 digits | — | — | "123" | — | INVALID |
| EmpNum 4 digits | — | — | "1234" | — | VALID |
| EmpNum 5 digits | — | — | "12345" | — | INVALID |
| Customer ID 8 digits | — | — | — | "12345678" | INVALID |
| Customer ID 9 digits | — | — | — | "123456789" | VALID |
| Customer ID 10 digits | — | — | — | "1234567890" | INVALID |

*Note: "abc12def" = 8 chars, 2 digits — valid. "abc123ef" = 8 chars, 3 digits — INVALID (digit rule).

---

## 7. Decision Table and Decision Tree

### Decision Table: `ValidatePassword()`

**Conditions:**
- C1: Length is 8–10 characters
- C2: Contains at least one letter
- C3: Contains at least one digit
- C4: Contains at least one special character (! # $ ,)

| Rule | C1 | C2 | C3 | C4 | Result |
|------|----|----|----|----|--------|
| R1 | F | — | — | — | INVALID (wrong length) |
| R2 | T | F | — | — | INVALID (no letter) |
| R3 | T | T | F | — | INVALID (no digit) |
| R4 | T | T | T | F | INVALID (no special char) |
| R5 | T | T | T | T | **VALID** ✅ |

**Total rules: 5** (covering all decision paths in the function).

---

### Decision Tree: `Login()` — Authentication Flow

```
                    ┌─────────────────────┐
                    │  User clicks LOGIN  │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ Username exists     │
                    │ in database?        │
                    └──────────┬──────────┘
               NO ─────────────┤──────────── YES
               │               │                │
        ┌──────▼──────┐        │        ┌───────▼───────┐
        │ Return FALSE│        │        │ Password hash │
        │ (not found) │        │        │ matches?      │
        └─────────────┘        │        └───────┬───────┘
                               │    NO ─────────┤───────── YES
                               │    │           │              │
                               │  ┌─▼────────┐  │       ┌──────▼──────────┐
                               │  │ Return   │  │       │ Return TRUE      │
                               │  │ FALSE    │  │       │ employee = found │
                               │  │(bad pwd) │  │       │ → open dashboard │
                               │  └──────────┘  │       └─────────────────┘
                               └────────────────┘
```

**Decision table for Login:**

| Username exists | Password matches | Result |
|----------------|-----------------|--------|
| F | — | Login FAILED — user not found |
| T | F | Login FAILED — wrong password |
| T | T | Login SUCCESS — session set |

---

## Summary of Testing Coverage

| Requirement | Covered by |
|---|---|
| 2 CFG functions | ValidateUsername + ValidatePassword |
| 3 User Stories | US-1 (Login), US-2 (Register Employee), US-3 (Customer) |
| 4 Test Cases | TC-US2-01/02 + TC-US3-01/02 |
| 2 Functional Tests | FT-1 (Auth), FT-2 (Customer validation) |
| 2 GUI Tests | GUI-1 (error label), GUI-2 (form clear) |
| Test scenario + 2 scripts | Employee Registration scenario, S-TC-01/02 |
| Boundary table (6 variables) | Username, Password, EmpNum, CustomerID, Weight |
| Decision table + tree | ValidatePassword (table) + Login (tree) |
