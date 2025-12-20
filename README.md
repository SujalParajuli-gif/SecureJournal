# SecureJournal — Personal Journaling Desktop App 

**Student:** Sujal Parajuli  
**London Met ID:** 23050262  

SecureJournal is a modern, secure, feature-rich **desktop journaling application UI** built using **.NET MAUI Blazor Hybrid**.  
This milestone focuses on the **full UI design and navigation** (no backend logic/database yet).

---

### Core Journal Entry UI
- One journal entry per day (UI flow + placeholders)
- Create / Update / Delete buttons (UI only)
- Date association (pick a day)
- System timestamps display (CreatedAt / UpdatedAt) (UI demo)
- Markdown editor + optional preview (UI demo)

### Mood Tracking UI
- Primary mood (required) and up to 2 secondary moods
- Mood categories: Positive / Neutral / Negative
- Mood list included as provided in the coursework brief

### Tags + Category UI
- Entry category dropdown (separate from mood category)
- Pre-built tags list
- Custom tag adding UI

### Navigation UI
- Sidebar navigation (dashboard-style)
- Calendar view (month grid, date selection, entry indicators)
- Timeline/List view with pagination UI

### Search & Filter UI
- Search input (title/content UI)
- Filters: date range, mood, tags (UI only)

### Streak Tracking UI
- Current streak / Longest streak / Missed days (UI placeholders)

### Dashboard Analytics UI (Date range filterable)
- Mood distribution (placeholder)
- Most frequent mood (placeholder)
- Most used tags (placeholder)
- Tag breakdown (placeholder)
- Word count trends (placeholder)


---

## Tech Stack
- **.NET MAUI Blazor Hybrid**
- **C# + Razor Components**
- **HTML/CSS**
- **Material Icons + Inter Font**
- Optional UI libs (if used): Tailwind via CDN (UI styling)

---

## Project Structure (Important Files)

- `Components/`
  - `Layout/` → MainLayout, Sidebar, TopBar
  - `Pages/` → Dashboard, Calendar, Timeline, Search, Streak, Analytics, Tags, Settings
  - `Shared/` → Reusable UI components (cards, pickers, calendar widget, pagination)
  - `Data/` → Constants (moods, tags, categories)
- `wwwroot/`
  - `index.html` → Blazor host page
  - `css/app.css` → App styling (dashboard theme)
  - `images/` → Dummy assets (e.g., wave illustration)

---

## Prerequisites

### Recommended (Visual Studio)
- **Visual Studio 2022** (latest)
- Workload: **.NET Multi-platform App UI development**
- Windows 10/11

### SDK
- **.NET SDK 8.x** (recommended for Windows target)

> If you plan to run Android/iOS later, you will also need Android SDK + emulators, etc. (not required for Milestone 1).

---

## Getting Started (Setup Guide)

### Option A — Run using Visual Studio (Recommended)
1. **Clone the repository**
   ```bash
   git clone https://github.com/SujalParajuli-gif/SecureJournal.git
   ```
2. Open the solution in Visual Studio:
   - Open `SecureJournal.slnx` (or `.sln` if present)
3. Ensure workloads are installed:
   - Visual Studio Installer → Modify → `.NET Multi-platform App UI development`
4. Restore + Build:
   - **Build → Rebuild Solution**
5. Run:
   - Select **Windows Machine**
   - Press **F5** (Start)

---

### Option B — Run using .NET CLI (Advanced)
> This works only if MAUI workloads are installed.

1. Go to the project folder:
   ```bash
   cd SecureJournal
   ```
2. Restore:
   ```bash
   dotnet restore
   ```
3. Build:
   ```bash
   dotnet build
   ```
4. Run (Windows target):
   ```bash
   dotnet run -f net8.0-windows10.0.19041.0
   ```

---


## License
This project is for individual academic coursework purposes.
