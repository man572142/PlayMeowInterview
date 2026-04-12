# Plan: Add Unit & Integration Tests

## Context
The project has zero tests. The Unity Test Framework (`com.unity.test-framework` 1.1.33) is installed but no test folders, assembly definitions, or test files exist. The goal is to add EditMode tests covering the scripts under `Assets/Scripts/`, using integration tests where unit testing is impractical.

## Minimal Production Code Changes

To make internal helpers testable from the test assembly without changing public API:

1. **`Assets/Scripts/PlayMeowInterview.asmdef`** — add `InternalsVisibleTo` for the test assembly via `overrideReferences` or by adding an `AssemblyInfo.cs`
2. **`Assets/Scripts/Network/GraphQLClient.cs`** — change `BuildRequestJson` from `private static` to `internal static` (line 77)
3. **`Assets/Scripts/Auth/AuthService.cs`** — change `ProcessAuthResponse` from `private` to `internal` (line 136), and add an `internal` constructor accepting `GraphQLClient` for DI in tests

These are visibility-only changes — no logic changes, no public API changes.

## Test Infrastructure Setup

Create:
- `Assets/Tests/EditMode/` directory
- `Assets/Tests/EditMode/PlayMeowInterview.Tests.EditMode.asmdef` — EditMode test assembly referencing `PlayMeowInterview` and `UnityEngine.TestRunner` / `UnityEditor.TestRunner`
- `Assets/Scripts/AssemblyInfo.cs` — `[assembly: InternalsVisibleTo("PlayMeowInterview.Tests.EditMode")]`

## Test Files

### 1. `LoginResultTests.cs` — Unit Tests
- `Ok_ReturnsSuccessWithToken` — verify Success=true, Token set, ErrorMessage null
- `Ok_WithUser_SetsUser` — verify User property populated
- `Fail_ReturnsFailureWithMessage` — verify Success=false, ErrorMessage set, Token null

### 2. `UserInfoTests.cs` — Unit Tests
- `Properties_CanBeSetAndRead` — round-trip Id and Username

### 3. `ProjectNoteDataTests.cs` — Unit Tests
- `DefaultName_IsNewNote` — verify default `name = "New Note"`
- `Description_DefaultsToNull` — verify uninitialized description

### 4. `GraphQLResponseTests.cs` — Unit Tests
- `HasErrors_NoErrors_ReturnsFalse`
- `HasErrors_NullErrors_ReturnsFalse`
- `HasErrors_WithErrors_ReturnsTrue`
- `FirstError_ReturnsMessage`
- `FirstError_NoErrors_ReturnsNull`

### 5. `GraphQLClientTests.cs` — Unit Tests (internal helpers)
- `JsonString_NullInput_ReturnsNull`
- `JsonString_PlainString_WrapsInQuotes`
- `JsonString_EscapesBackslash`
- `JsonString_EscapesQuotes`
- `JsonString_EscapesNewlineAndTab`
- `BuildRequestJson_QueryOnly_ProducesValidJson`
- `BuildRequestJson_WithVariables_IncludesVariablesObject`

### 6. `ProjectNoteBookTests.cs` — Unit Tests (ScriptableObject)
Uses `ScriptableObject.CreateInstance<ProjectNoteBook>()` — works in EditMode.
- `GetNoteNames_EmptyList_ReturnsSingleNoNotesEntry`
- `GetNoteNames_NullList_ReturnsSingleNoNotesEntry`
- `GetNoteNames_PopulatedList_ReturnsNames`
- `GetNoteNames_EmptyName_ReturnsUnnamedPlaceholder`

### 7. `TokenStoreTests.cs` — Integration Tests (PlayerPrefs)
Uses real PlayerPrefs with cleanup in `[TearDown]`.
- `SaveAndLoad_RoundTrips`
- `Load_NoToken_ReturnsNull`
- `HasToken_AfterSave_ReturnsTrue`
- `Clear_RemovesToken`

### 8. `AuthServiceTests.cs` — Unit + Integration Tests
Uses the internal constructor with a real `GraphQLClient` (no network calls for validation tests).
- `LoginAsync_EmptyUsername_ReturnsFail`
- `LoginAsync_EmptyPassword_ReturnsFail`
- `GoogleLoginAsync_ReturnsNotImplementedFail`
- `Logout_ClearsTokenAndUser`
- `ProcessAuthResponse_NetworkError_ReturnsFail`
- `ProcessAuthResponse_GraphQLErrors_ReturnsFail`
- `ProcessAuthResponse_ValidResponse_ReturnsOkWithToken`
- `ProcessAuthResponse_EmptyToken_ReturnsFail`

## Files to Create/Modify

| Action | Path |
|--------|------|
| Create | `Assets/Scripts/AssemblyInfo.cs` |
| Create | `Assets/Tests/EditMode/PlayMeowInterview.Tests.EditMode.asmdef` |
| Create | `Assets/Tests/EditMode/LoginResultTests.cs` |
| Create | `Assets/Tests/EditMode/UserInfoTests.cs` |
| Create | `Assets/Tests/EditMode/ProjectNoteDataTests.cs` |
| Create | `Assets/Tests/EditMode/GraphQLResponseTests.cs` |
| Create | `Assets/Tests/EditMode/GraphQLClientTests.cs` |
| Create | `Assets/Tests/EditMode/ProjectNoteBookTests.cs` |
| Create | `Assets/Tests/EditMode/TokenStoreTests.cs` |
| Create | `Assets/Tests/EditMode/AuthServiceTests.cs` |
| Modify | `Assets/Scripts/Network/GraphQLClient.cs` (line 77: private→internal) |
| Modify | `Assets/Scripts/Auth/AuthService.cs` (line 136: private→internal; add internal ctor) |

## Verification
1. Open Unity Editor → Window → General → Test Runner → EditMode tab
2. All tests should appear and pass
3. Or via CLI: `Unity.exe -runTests -projectPath . -testResults results.xml -testPlatform EditMode`
