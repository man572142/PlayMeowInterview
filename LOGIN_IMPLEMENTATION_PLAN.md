# Login UI Implementation Plan
## PlayMeow Interview — "Login 登入頁" Figma → Unity
---

## Design Summary

The Figma frame (720×1561 mobile portrait) is a complete login screen containing:

| Element | Type | Size | Details |
|---|---|---|---|
| **Background** | Image | 720×1561 | Full-bleed illustration (`bg-login.png`) |
| **Logo** | Image | 640×199 | PlayMeow brand asset (`logo.png`) |
| **Heading** | Text | 24px | "請輸入帳號密碼" — Noto Sans TC, white, centered |
| **Email Input** | Text Field | 520×56 | Dark bg `#3B3B3B`, pink border `#FF678F` (2px), rounded 56px |
| **Password Input** | Text Field | 520×56 | Dark bg `#3B3B3B`, no border, rounded 56px, ContentType=Password |
| **Error Message** | Text | 24px | "帳號或密碼錯誤" — `#F33366` (red), centered, hidden by default |
| **Login Button** | Button | 520×56 | "登入" — bg pink `#FF678F`, 32px white text, rounded 56px |
| **Links Row** | Text (2×) | — | "忘記密碼" (left) / "註冊新帳號" (right) — `#FF678F` |
| **Divider Text** | Text | 27px | "或透過以下方式繼續" — white, centered |
| **Google Button** | Button | 520×56 | White bg, Google logo centered, rounded 56px |
| **Footer** | Text (2×) | — | "隱私權" (gradient `#FFA3A0`→`#FEC5E1`) / "服務條款" (`#F4B3C2`) |
| **Close Button** | Button | 44×44 | X icon (`icon-close.png`), top-right corner |

### Color Palette
- **Primary Accent:** `#FF678F` (pink1) — borders, login button, interactive links
- **Secondary Accent:** `#F4B3C2` (pink2) — footer secondary text
- **Error:** `#F33366` (red)
- **Input Background:** `#3B3B3B`
- **White:** `#FFFFFF`
- **Gradient:** `#FFA3A0` → `#FEC5E1` (footer privacy link)

### Typography
- **Heading & Form Labels:** Noto Sans TC, Regular, 24px, white
- **Input Placeholder:** Inter/Noto Sans JP, Regular, 24px, 40% opacity white
- **Button Text:** Inter/Noto Sans JP, Regular, 32px, white
- **Footer:** Noto Sans TC, Regular, 24px
- **Divider:** Noto Sans TC, Regular, 27px

---

## Asset Downloads

All assets must be downloaded from Figma and stored in `Assets/Arts/Images/Login/`.

| Filename | Figma URL | Dimensions | Usage |
|---|---|---|---|
| `bg-login.png` | `https://www.figma.com/api/mcp/asset/81a3caae-333a-4911-83d4-9bbe2142df14` | 720×1561 | Background image (full-bleed) |
| `logo.png` | `https://www.figma.com/api/mcp/asset/d7fca7fb-bba2-4b4b-b69e-8b99058785c4` | 640×199 | PlayMeow logo |
| `google-logo.png` | `https://www.figma.com/api/mcp/asset/b09a6420-37f3-43ef-81f0-a07aa9d1dbd4` | 127×42 | Google sign-in logo |
| `icon-close.png` | `https://www.figma.com/api/mcp/asset/39af7eb8-7c9c-4497-adfd-e7e0ef5b6385` | 44×44 | Close button icon |

**Import Settings:**
- **Texture Type:** Sprite (2D and UI)
- **Sprite Mode:** Single
- **Pixels Per Unit:** 100
- **Filter Mode:** Bilinear (or Point for pixel-perfect look)

---

## Subagent A: Asset Export & Download

**Objective:** Download all image assets from Figma URLs and import them into the Unity project.

**Deliverables:**
1. `Assets/Arts/Images/Login/bg-login.png` — background illustration
2. `Assets/Arts/Images/Login/logo.png` — PlayMeow brand logo
3. `Assets/Arts/Images/Login/google-logo.png` — Google sign-in logo  
4. `Assets/Arts/Images/Login/icon-close.png` — close button X icon
5. **Bonus:** Generate/create a rounded rectangle sprite `pill-dark.png` (520×56, dark with rounded corners) for use as a background for input fields

**Process:**
- Fetch each image from the Figma API URL (valid for 7 days from export)
- Save to correct Unity folder structure
- Apply appropriate import settings (Sprite, 100 PPU, Bilinear filter)
- Create or source a rounded rectangle sprite for input field backgrounds (alternative: use 9-sliced sprite, or use a tinted Image component with rounded corners via materials)

**Dependencies:** None — can execute immediately.

**Parallel Work:** Yes — start simultaneously with Subagents B and C.

---

## Subagent B: C# UI Scripts

**Objective:** Write all C# scripts for login UI logic, authentication, and networking.

**Deliverables:**

 1. Assets/Scripts/UI/LoginView.cs — MonoBehaviour that:
    - Holds references to all UI elements (TMP_InputField for email/password, Buttons, TextMeshProUGUI for error text)
    - Exposes public methods: OnLoginClicked(), OnGoogleLoginClicked(), OnForgotPasswordClicked(), OnRegisterClicked(), OnCloseClicked()
    - Shows/hides error message via ShowError(string msg) / HideError()
    - Delegates authentication calls to AuthService
  2. Assets/Scripts/Auth/AuthService.cs — Singleton/static service that:
    - Exposes async Task<LoginResult> LoginAsync(string email, string password)
    - Exposes async Task<LoginResult> GoogleLoginAsync()
    - Calls the GraphQL endpoint via GraphQLClient
    - Returns a LoginResult (success/failure + error message + token)
  3. Assets/Scripts/Network/GraphQLClient.cs — Lightweight HTTP utility that:
    - Sends POST requests with JSON body to a configurable GraphQL endpoint
    - Uses UnityWebRequest
    - Returns deserialized response or error
  4. Assets/Scripts/Auth/LoginResult.cs — Simple data class holding bool Success, string ErrorMessage, string Token

**Dependencies:** None — scripts are self-contained and don't depend on existing project assets.

**Parallel Work:** Yes — start simultaneously with Subagents A and C.

## Subagent C: UI Layout Specification

**Objective:** Produce a detailed, pixel-accurate specification for the UGUI Canvas hierarchy that Subagent D will implement.

  Deliverables: A structured document describing the full Canvas hierarchy, anchoring, sizing, colors, fonts, and component assignments:

  Canvas (ScreenSpace-Overlay, CanvasScaler: ScaleWithScreenSize 720×1560)
  └── Panel_Background (Image: bg-login, stretched)
  └── Panel_Center (anchored center, 520px wide, VerticalLayoutGroup gap=40)
      ├── Group_Account (VerticalLayoutGroup gap=24, center-aligned)
      │   ├── Image_Logo (640×199, logo.png)
      │   ├── TMP_Text "請輸入帳號密碼" (24pt, white, center)
      │   ├── InputField_Email (520×56, rounded bg #3B3B3B, border #FF678F 2px)
      │   ├── InputField_Password (520×56, rounded bg #3B3B3B, no border, ContentType=Password)
      │   ├── TMP_ErrorText "帳號或密碼錯誤" (24pt, #F33366, center, hidden by default)
      │   ├── Button_Login (520×56, bg #FF678F, "登入" 32pt white)
      │   └── Group_Links (horizontal, space-between)
      │       ├── TMP_ForgotPassword "忘記密碼" (#FF678F)
      │       └── TMP_Register "註冊新帳號" (#FF678F)
      └── Group_Service (VerticalLayoutGroup gap=24, center-aligned)
          ├── TMP_Text "或透過以下方式繼續" (27pt, white)
          └── Button_Google (520×56, bg white, Google logo centered)
  └── Panel_Footer (anchored bottom, horizontal, "隱私權" + "服務條款")
  └── Button_Close (anchored top-right, 44×44, icon-close.png)

**Dependencies:** None — this is a specification document.

**Parallel Work:** Yes — start simultaneously with Subagents A and B.

---

## Subagent D: Prefab & Scene Assembly (Unity MCP)

**Objective:** Use the `unity-mcp-skill` to build the Canvas hierarchy in the scene, wire up all components, and save as a prefab.

**Deliverables:**

1. **`Assets/Scenes/SampleScene.unity`** — Updated scene with fully assembled login UI hierarchy
2. **`Assets/Prefabs/LoginCanvas.prefab`** — Reusable prefab of the complete login UI
3. All Image components with sprites properly assigned (from Subagent A)
4. All Button and InputField components fully configured (per Subagent C spec)
5. LoginView script attached to Canvas with serialized field references wired up
6. CanvasScaler configured for mobile (Scale With Screen Size, 720×1560 reference)

**Process:**

1. **Create Canvas:**
   - ScreenSpace-Overlay mode
   - Apply CanvasScaler (Scale With Screen Size, reference 720×1560)
   - Apply GraphicRaycaster

2. **Import Sprites:**
   - Load all assets from `Assets/Arts/Images/Login/` (from Subagent A)
   - Create Image components with proper sprite assignments

3. **Build Hierarchy:**
   - Programmatically instantiate the full UGUI hierarchy (per Subagent C spec)
   - Use Unity MCP tools to create GameObjects, add components, set properties
   - Apply layout groups, anchoring, and sizing

4. **Configure Components:**
   - Set up InputFields (email, password) with content type and placeholder text
   - Attach Buttons with OnClick() listeners wired to LoginView methods
   - Configure TextMeshProUGUI components with correct fonts, sizes, colors, alignment

5. **Attach Scripts:**
   - Add LoginView component to Canvas
   - Serialize all field references in the Inspector (InputFields, Buttons, TextMeshProUGUI)
   - Verify script compilation before attaching

6. **Save Prefab:**
   - Save the fully assembled Canvas as `Assets/Prefabs/LoginCanvas.prefab`
   - Verify prefab integrity (all serialized fields intact)

7. **Validate:**
   - Use Unity MCP to take a screenshot of the scene
   - Compare visual output to Figma design
   - Adjust colors, spacing, fonts if necessary

**Dependencies:**
- **Must complete after Subagent A** — sprites must exist in project
- **Must complete after Subagent B** — scripts must compile before attaching
- **Must complete after Subagent C** — layout spec drives the hierarchy build
- **Must complete after all Phase 1 subagents** — integration task that depends on all inputs

**Parallel Work:** No — must run sequentially after Phase 1.

**Notes:**
- Use `unity-mcp-skill` to automate all GameObject creation and property assignment
- Reference node IDs from the Figma design in comments (for future updates)
- Ensure all serialized fields are properly wired (no missing references)
- Test input interactions (focus, text entry, validation) manually after assembly

---

## Execution Roadmap

### Phase 1: Parallel Asset, Script, and Specification Development
**Duration:** ~1–2 hours (estimated)  
**Subagents:** A, B, C (all independent)

```
START
  ├─→ Subagent A: Download and import assets
  ├─→ Subagent B: Write C# scripts (UI, Auth, Network)
  └─→ Subagent C: Generate UI layout specification
         │         (outputs detailed Canvas hierarchy doc)
         │
         └─→ [Wait for all 3 to complete]
```

**Success Criteria:**
- [ ] All 4 image assets downloaded and imported as sprites
- [ ] All 6 C# scripts written, placed in correct directories, compile without errors
- [ ] Detailed UI specification document completed with full Canvas hierarchy

### Phase 2: Integration via Unity MCP
**Duration:** ~1–2 hours (estimated)  
**Subagent:** D (depends on A, B, C completion)

```
   Phase 1 Complete
         │
         ▼
   Subagent D: Build Canvas and Prefab
         ├─→ Create Canvas + CanvasScaler
         ├─→ Import sprites and build hierarchy
         ├─→ Attach scripts and wire serialized fields
         ├─→ Screenshot and validate
         └─→ Save prefab
              │
              ▼
         COMPLETE
```

**Success Criteria:**
- [ ] Canvas hierarchy fully built in SampleScene
- [ ] LoginCanvas.prefab created and verified
- [ ] All serialized fields properly wired (no missing references)
- [ ] Visual output matches Figma design (colors, spacing, fonts, layout)
- [ ] Scripts compile and attach without errors
- [ ] Input fields respond to user interaction (focus, text entry)
- [ ] Buttons are clickable and trigger LoginView methods

---

## Risk Mitigation

### Risk 1: Font Availability
**Issue:** Noto Sans TC and Inter may not be available as TMP fonts.  
**Mitigation:** 
- Subagent B should check `listAvailableFontsAsync()` and document actual font names
- Fall back to default TMP font (Arial) if unavailable
- Update specification (Subagent C) with actual font choices
- Note as a follow-up: import TMP font assets for Noto Sans TC and Inter if needed

### Risk 2: Rounded Rectangle Sprites
**Issue:** UGUI doesn't natively support rounded corners; requires 9-sliced or custom sprites.  
**Mitigation:**
- Subagent A: Generate or source 9-sliced sprites for pill shapes
- Alternative: use a tinted Image component with a rounded-corner material
- Document the chosen approach in Subagent D output

### Risk 3: Gradient Text (Privacy Link)
**Issue:** TMP gradient text requires proper material and vertex color setup.  
**Mitigation:**
- Subagent D: Use TextMeshProUGUI with `VertexGradient` property
- Verify gradient renders correctly in scene view before saving prefab
- If gradient doesn't work, fall back to solid color (#FFA3A0) and note as follow-up

### Risk 4: GraphQL Endpoint Not Known
**Issue:** AuthService and GraphQLClient need a real endpoint URL.  
**Mitigation:**
- Subagent B: Hardcode a placeholder URL (e.g., `https://api.example.com/graphql`) 
- Or create a ScriptableObject config for the endpoint
- Document in script comments that this must be updated with the real endpoint before testing

### Risk 5: InputField Placeholder Text
**Issue:** Placeholder text in Figma shows actual placeholder ("a@b.com" for email, "請輸入密碼" for password).  
**Mitigation:**
- Subagent D: Use InputField's built-in placeholder mechanism (TMP_Text child)
- Or handle placeholder visually via Text overlay and script logic
- Keep placeholder opacity at 40% for password field as per design

---

