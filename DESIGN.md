# Konekto Design System Specification (DESIGN.md)

> **Design Archetype:** Modern SaaS & University Community (Inspired by [styles.refero.design](https://styles.refero.design) & [VoltAgent/awesome-design-md](https://github.com/VoltAgent/awesome-design-md))  
> **Aesthetic Philosophy:** Clean, High-Trust, Functional Elegance (Stripe & Linear influenced)  
> **Primary Technology Stack:** ASP.NET Core Razor Views, Bootstrap 5.3, Font Awesome 6, Plus Jakarta Sans  

This `DESIGN.md` serves as the **Visual Source of Truth** for developers, UI designers, and AI coding agents working on the **Konekto** platform. When generating or refactoring views, components, and user interfaces, strictly adhere to the tokens, component patterns, and visual directives detailed below.

---

## 1. Design Philosophy & Creative North Star

Konekto bridges academic heritage with modern enterprise SaaS aesthetics. Interfaces must feel:
1. **Credible & Authoritative:** High-contrast typography and clean structural borders inspired by academic institutions.
2. **Effortless & Accessible:** Generous whitespace, intuitive 8pt spatial rhythm, and clear visual hierarchy.
3. **Kinetic & Engaging:** Subtle micro-interactions, pill-shaped action badges, and refined hover elevations (`translateY(-3px)`).
4. **Resilient & Consistent:** Zero visual clutter, consistent component padding, and standardized semantic states.

---

## 2. Color Palette & Design Tokens

### 2.1 Brand & Accent Tokens
```css
:root {
  /* Brand Core */
  --color-primary:          #4F46E5; /* Indigo 600 - Main Brand Anchor */
  --color-primary-hover:    #4338CA; /* Indigo 700 - Active / Hover */
  --color-primary-light:    #EEF2FF; /* Indigo 50  - Subtle Backgrounds */
  --color-primary-glow:     rgba(79, 70, 229, 0.25);

  /* Secondary Accents */
  --color-accent-blue:      #3B82F6; /* Blue 500   - Career & Links */
  --color-accent-purple:    #8B5CF6; /* Purple 500 - Gradients & Highlights */
}
```

### 2.2 Neutral & Surface Tokens (Slate / Zinc Scale)
```css
:root {
  --color-bg-canvas:        #F8FAFC; /* Slate 50  - Page Background */
  --color-bg-surface:       #FFFFFF; /* White     - Card & Modal Surface */
  --color-bg-subtle:        #F1F5F9; /* Slate 100 - Secondary Fill / Inputs */
  
  --color-border-subtle:    #E2E8F0; /* Slate 200 - Default Card Borders */
  --color-border-hover:     #CBD5E1; /* Slate 300 - Interactive Hover */

  --color-text-heading:     #0F172A; /* Slate 900 - Primary Headings */
  --color-text-body:        #334155; /* Slate 700 - Reading Copy */
  --color-text-muted:       #64748B; /* Slate 500 - Metadata & Captions */
  --color-text-disabled:    #94A3B8; /* Slate 400 - Disabled / Inactive */
}
```

### 2.3 Semantic & Functional Tokens
```css
:root {
  /* Success / Career / Active */
  --color-success:          #10B981; /* Emerald 500 */
  --color-success-bg:       #ECFDF5; /* Emerald 50 */
  --color-success-text:     #065F46; /* Emerald 800 */

  /* Warning / Events / Spotlight */
  --color-warning:          #F59E0B; /* Amber 500 */
  --color-warning-bg:       #FFFBEB; /* Amber 50 */
  --color-warning-text:     #92400E; /* Amber 800 */

  /* Danger / Errors / 404 & 5xx */
  --color-danger:           #EF4444; /* Red 500 */
  --color-danger-bg:        #FEF2F2; /* Red 50 */
  --color-danger-text:      #991B1B; /* Red 800 */

  /* Info / Academic / Announcements */
  --color-info:             #0EA5E9; /* Sky 500 */
  --color-info-bg:          #F0F9FF; /* Sky 50 */
  --color-info-text:        #075985; /* Sky 800 */
}
```

---

## 3. Typography Scale & Hierarchy

* **Primary Typeface:** `Plus Jakarta Sans`, sans-serif (Google Fonts)
* **Font Smoothing:** `-webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale;`

| Level | Size | Weight | Line Height | Tracking | Usage |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Hero Display** | 2.50rem (40px) | 800 (ExtraBold) | 1.15 | -0.03em | Homepage Hero headline |
| **Heading 1** | 2.00rem (32px) | 800 (ExtraBold) | 1.25 | -0.02em | Section titles, main page headers |
| **Heading 2** | 1.50rem (24px) | 700 (Bold) | 1.30 | -0.015em| Modal titles, prominent card titles |
| **Heading 3** | 1.25rem (20px) | 700 (Bold) | 1.35 | -0.01em | Announcement & job card titles |
| **Heading 4** | 1.05rem (17px) | 600 (SemiBold) | 1.40 | 0.00em | Profile names, widget headers |
| **Body Large** | 1.00rem (16px) | 400 / 500 | 1.60 | 0.00em | Lead paragraphs, introduction text |
| **Body Regular**| 0.925rem (15px)| 400 (Regular) | 1.55 | 0.00em | Default body text, card content |
| **Microcopy** | 0.825rem (13px)| 500 / 600 | 1.40 | +0.01em| Dates, authors, pill badges |

---

## 4. Spacing, Elevation & Corner Radii

### 4.1 8-Point Spatial System
All margin and padding allocations must follow 4px / 8px multiples:
* `space-1`: 4px (`2xs`)
* `space-2`: 8px (`xs`)
* `space-3`: 12px (`sm`)
* `space-4`: 16px (`md` - standard gutter)
* `space-5`: 20px
* `space-6`: 24px (`lg` - standard card padding)
* `space-8`: 32px (`xl` - section spacing)
* `space-12`: 48px (`2xl` - large container spacing)
* `space-16`: 64px (`3xl` - hero section vertical rhythm)

### 4.2 Corner Radii (Border Radius)
* **Pill / Circular:** `50px` / `9999px` (`.rounded-pill` for badges, search boxes, and primary CTA buttons)
* **Large Containers / Hero:** `24px` (`border-radius: 24px;`)
* **Cards & Panels:** `14px` – `18px` (`border-radius: 16px;`)
* **Inputs & Selects:** `10px` (`border-radius: 10px;`)
* **Avatars:** `50%` (`rounded-circle`)

### 4.3 Elevation & Shadow Scales
```css
/* Subtle Card Default */
--shadow-card: 0 1px 3px rgba(0, 0, 0, 0.04), 0 1px 2px rgba(0, 0, 0, 0.02);

/* Card Hover Elevation */
--shadow-hover: 0 12px 28px -6px rgba(15, 23, 42, 0.08), 0 4px 10px -2px rgba(15, 23, 42, 0.03);

/* Hero & Floating Search Bar */
--shadow-float: 0 20px 40px -15px rgba(79, 70, 229, 0.35);

/* Primary Button Glow */
--shadow-btn: 0 4px 14px rgba(79, 70, 229, 0.35);
```

---

## 5. Refero-Inspired Component Catalog

### 5.1 Hero Search Bar Pattern
```html
<form class="hero-search-container d-flex align-items-center">
  <span class="ps-3 text-muted"><i class="fa-solid fa-magnifying-glass"></i></span>
  <input type="text" class="form-control hero-search-input" placeholder="Search alumni, companies, or titles..." />
  <button type="submit" class="btn btn-konekto rounded-pill px-4 py-2">Search</button>
</form>
```
* **Style:** Floating frosted pill container with `backdrop-filter: blur(10px);`, zero-border inner input, and vibrant right-aligned action button.

### 5.2 Interactive Cards (`card-custom`)
```html
<div class="card card-custom p-4 h-100">
  <div class="d-flex justify-content-between align-items-center mb-2">
    <span class="badge bg-primary-subtle text-primary fw-bold">Career</span>
    <small class="text-muted"><i class="fa-regular fa-calendar me-1"></i> Date</small>
  </div>
  <h5 class="fw-bold text-dark mb-2">Title</h5>
  <p class="text-muted small flex-grow-1">Content description...</p>
  <div class="d-flex justify-content-between align-items-center mt-3 pt-3 border-top">
    <small class="text-secondary"><i class="fa-regular fa-user me-1"></i> Author</small>
    <a href="#" class="btn btn-sm btn-outline-primary rounded-pill">Detail</a>
  </div>
</div>
```
* **Behavior:** Smooth 250ms transition. On `:hover`, elevates with `transform: translateY(-3px)` and dynamic border highlight.

### 5.3 Buttons System
* **Primary (`.btn-konekto`):** Background `#4F46E5`, white text, 600 weight, subtle indigo shadow on hover.
* **Secondary Outline:** Transparent background, `1px solid #CBD5E1`, text `#334155`, hover background `#F8FAFC`.
* **Subtle Ghost:** Borderless, text `#4F46E5`, hover background `#EEF2FF`.

### 5.4 Semantic Badges & Category Tags
Badges use the **pastel background + saturated text** modern formula:
* **Career:** `badge bg-success-subtle text-success`
* **Events:** `badge bg-warning-subtle text-dark`
* **Academic:** `badge bg-info-subtle text-info`
* **General:** `badge bg-primary-subtle text-primary`

---

## 6. Micro-Interactions & Animation Guidelines

```css
/* Universal Smooth Transitions */
.card-custom, .btn, .stat-card, .feature-card {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Hover States */
.card-custom:hover {
  transform: translateY(-3px);
  border-color: var(--color-border-hover);
  box-shadow: var(--shadow-hover);
}

.stat-card:hover {
  transform: translateY(-4px);
  border-color: #6366F1;
}
```

---

## 7. Accessibility (WCAG 2.1 AA) Rules

1. **Color Contrast:** All body text must maintain at least a **4.5:1** contrast ratio against its background. Large titles (24px+) must maintain at least **3.0:1**.
2. **Focus Visibility:** Interactive elements must display a distinct focus ring:
   ```css
   :focus-visible {
     outline: 2px solid var(--color-primary);
     outline-offset: 2px;
   }
   ```
3. **Semantic Markup:** Always utilize semantic HTML tags (`<header>`, `<nav>`, `<main>`, `<section>`, `<footer>`, `<article>`).
4. **Form Labels:** Every `<input>` must have a programmatic `<label>` or `aria-label`.

---

## 8. AI Prompt Directives (Rules for AI Coding Agents)

When an AI agent (Cursor, Claude, Copilot) is tasked with creating new pages or components for Konekto:
1. **Never use generic browser defaults:** Always apply `Plus Jakarta Sans`, standard card border radii (`14px`–`18px`), and subtle borders (`#E2E8F0`).
2. **Always include empty states:** If a collection or table can be empty (`!Model.Any()`), provide an illustration/icon card with helpful guidance.
3. **Respect the color token system:** Do not invent arbitrary hex codes. Utilize the predefined Primary Indigo, Neutral Slate, and Semantic palettes.
4. **Enforce Responsive Layouts:** Use Bootstrap 5 grid systems (`col-12 col-md-6 col-lg-4`) with mobile-first breakpoints.
5. **Ensure Icon Consistency:** Use Font Awesome 6 icons with appropriate semantic meaning (e.g., `fa-bullhorn` for announcements, `fa-briefcase` for jobs, `fa-user-graduate` for alumni).
