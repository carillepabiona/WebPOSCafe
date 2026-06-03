let activeCategory = '@Model.ActiveCategory';

function filterByCategory(catId) {
    activeCategory = catId;

    document.querySelectorAll('.md-cat-btn').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.cat === catId);
    });

    document.querySelectorAll('.md-section').forEach(section => {
        section.style.display =
            (catId === 'all' || section.dataset.catId === catId) ? '' : 'none';
    });

    searchItems();
}

function searchItems() {
    const q = document.getElementById('mdSearch').value.toLowerCase().trim();

    document.querySelectorAll('.md-item-card').forEach(card => {
        const name = card.dataset.name || '';
        const catId = card.dataset.cat || '';
        const catOk = activeCategory === 'all' || catId === activeCategory;
        const nameOk = !q || name.includes(q);
        card.style.display = (catOk && nameOk) ? '' : 'none';
    });

    document.querySelectorAll('.md-section').forEach(section => {
        const cid = section.dataset.catId;
        if (activeCategory !== 'all' && cid !== activeCategory) {
            section.style.display = 'none';
            return;
        }
        const visible = section.querySelectorAll('.md-item-card:not([style*="display: none"])');
        section.style.display = visible.length > 0 ? '' : 'none';
    });
}

// Real data injected from the Razor page
const MENU = MENU_DATA.flatMap(cat =>
    cat.items.map(item => ({ ...item, cat: cat.id, catName: cat.name, catColor: cat.color }))
);