'use strict';

const REGIME_COLORS = {
    'Photosynthèse': '#2e9e6b',
    'Herbivore': '#3266ad',
    'Carnivore': '#e07b39',
    'Omnivore': '#f0b429',
    'Coprophages': '#8e6bbf',
    'Limivores': '#8e6bbf',
    'Detritivores': '#8e6bbf',
    'Heterotrophie': '#8e6bbf'
};

const TYPE_PALETTE = [
    '#4e8eca', '#3266ad', '#2b6cb0', '#63b3ed', '#4299e1',
    '#2196f3', '#1976d2', '#0288d1', '#00796b', '#2e7d32',
    '#388e3c', '#558b2f', '#f57c00', '#e65100', '#6a1b9a',
    '#ad1457', '#c62828', '#37474f', '#455a64', '#546e7a'
];

function formatPct(val, total) {
    if (total === 0) return '0%';
    const pct = val * 100 / total;
    if (pct === 0) return '0%';
    if (pct < 0.1) return '< 0.1%';
    if (pct < 1) return pct.toFixed(1) + '%';
    return Math.round(pct) + '%';
}

function exportChart(canvasId, filename) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const link = document.createElement('a');
    link.download = filename + '.png';
    link.href = canvas.toDataURL('image/png');
    link.click();
}

function renderLegend(containerId, labels, colors) {
    const el = document.getElementById(containerId);
    if (!el) return;
    el.innerHTML = labels.map((label, i) =>
        `<span class="d-inline-flex align-items-center gap-1">
            <span style="width:13px;height:13px;background:${colors[i % colors.length]};
                  border-radius:3px;flex-shrink:0;display:inline-block;"></span>
            <small>${label}</small>
        </span>`
    ).join('');
}

function showEmpty(canvasId, message) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    canvas.parentElement.innerHTML =
        `<p class="text-muted text-center py-5">${message}</p>`;
}

// ─── Point d'entrée ────────────────────────────────────────────────────────

function initBiomeCharts(data) {
    if (!data || !data.species || data.species.length === 0) {
        showEmpty('chart-bar', 'Aucune espèce dans ce biome.');
        showEmpty('chart-doughnut', 'Aucune espèce dans ce biome.');
        showEmpty('chart-radar', 'Aucune espèce dans ce biome.');
        return;
    }
    initBarChart(data);
    initDoughnutChart(data);
    initRadarChart(data);
}

// ─── Bar : espèces par type ─────────────────────────────────────────────────

let _barChart = null;
let _barData = null;
let _barMode = 'count'; // 'count' | 'mass'

function setBarMode(mode) {
    _barMode = mode;
    document.getElementById('btn-mode-count').classList.toggle('active', mode === 'count');
    document.getElementById('btn-mode-mass').classList.toggle('active', mode === 'mass');
    if (_barData) {
        if (_barChart) { _barChart.destroy(); _barChart = null; }
        _buildBarChart(_barData);
    }
}

function initBarChart(data) {
    _barData = data;
    _buildBarChart(data);
}

function _showBarDetail(type) {
    const isMass = _barMode === 'mass';
    const species = _barData.species.filter(s => s.type === type);
    const detail = document.getElementById('detail-bar');
    detail.classList.remove('d-none');
    detail.innerHTML =
        `<strong>${type}</strong>
         <ul class="mb-0 mt-1">
           ${species.map(s => {
            const extra = isMass
                ? `masse : ${(s.quantity * s.weight).toFixed(1)} kg`
                : `qté : ${s.quantity}`;
            return `<li>${s.name} — régime : ${s.regime} — ${extra}</li>`;
        }).join('')}
         </ul>`;
}

function _buildBarChart(data) {
    const isMass = _barMode === 'mass';
    const typeMap = {};
    data.species.forEach(s => {
        const val = isMass ? s.quantity * s.weight : s.quantity;
        typeMap[s.type] = (typeMap[s.type] || 0) + val;
    });

    const labels = Object.keys(typeMap);
    const values = Object.values(typeMap);
    const colors = labels.map((_, i) => TYPE_PALETTE[i % TYPE_PALETTE.length]);
    const total = values.reduce((a, b) => a + b, 0);

    renderLegend('legend-bar', labels, colors);

    // Badges cliquables sous le chart — accessibles même pour les barres invisibles
    const summary = document.getElementById('bar-summary');
    if (summary) {
        summary.innerHTML = labels.map((label, i) => {
            const val = values[i];
            const pct = formatPct(val, total);
            const formatted = isMass
                ? (val >= 1000 ? `${(val / 1000).toFixed(1)} t` : `${val.toFixed(0)} kg`)
                : val;
            return `<button type="button"
                        class="btn btn-sm rounded-pill px-2 py-0"
                        style="background:${colors[i]};color:#fff;font-size:0.75rem;"
                        onclick="_showBarDetail('${label.replace(/'/g, "\\'")}')"
                        title="${pct} du total">
                        ${label} : ${formatted}
                    </button>`;
        }).join('');
    }

    // Plugin inline : affiche la valeur au-dessus de chaque barre
    const valueLabelPlugin = {
        id: 'valueLabels',
        afterDatasetsDraw(chart) {
            const { ctx, data: cd, scales: {y} } = chart;
            const meta = chart.getDatasetMeta(0);
            ctx.save();
            ctx.font = 'bold 11px sans-serif';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'bottom';
            ctx.fillStyle = '#333';
            meta.data.forEach((bar, i) => {
                const val = cd.datasets[0].data[i];
                if (!val) return;
                const label = _barMode === 'mass'
                    ? (val >= 1000 ? `${(val / 1000).toFixed(1)} t` : `${val.toFixed(0)} kg`)
                    : val;
                const yPos = Math.max(bar.y - 3, chart.chartArea.top + 12);
                ctx.fillText(label, bar.x, yPos);
            });
            ctx.restore();
        }
    };

    const canvas = document.getElementById('chart-bar');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    _barChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                label: isMass ? 'Masse totale (kg)' : 'Quantité totale',
                data: values,
                backgroundColor: colors,
                borderRadius: 4,
                minBarLength: 6
            }]
        },
        plugins: [valueLabelPlugin],
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {padding: {top: 20}},
            plugins: {
                legend: {display: false},
                tooltip: {
                    callbacks: {
                        afterBody(items) {
                            const type = _barChart.data.labels[items[0].dataIndex];  
                            return _barData.species                                    
                                .filter(s => s.type === type)
                                .map(s => _barMode === 'mass'                        
                                    ? `  • ${s.name} — ${(s.quantity * s.weight).toFixed(1)} kg`
                                    : `  • ${s.name} ×${s.quantity}`);
                        }
                    }
                }
            },
            onClick(_event, elements) {
                if (!elements.length) return;
                _showBarDetail(labels[elements[0].index]);
            }
        }
    });
}

// ─── Doughnut : régimes alimentaires ───────────────────────────────────────

let _doughnutChart = null;
let _doughnutData = null;
let _doughnutMode = 'count'; // 'count' | 'mass'

function setDoughnutMode(mode) {
    _doughnutMode = mode;
    document.getElementById('btn-donut-count').classList.toggle('active', mode === 'count');
    document.getElementById('btn-donut-mass').classList.toggle('active', mode === 'mass');
    if (_doughnutData) {
        if (_doughnutChart) { _doughnutChart.destroy(); _doughnutChart = null; }
        _buildDoughnutChart(_doughnutData);
    }
}

function _showDoughnutDetail(regime) {
    const isMass = _doughnutMode === 'mass';
    const species = _doughnutData.species.filter(s => s.regime === regime);

    document.querySelectorAll('#legend-doughnut button').forEach(btn => {
        btn.style.opacity = btn.dataset.regime === regime ? '1' : '0.45';
        btn.style.fontWeight = btn.dataset.regime === regime ? 'bold' : 'normal';
    });

    const detail = document.getElementById('detail-doughnut');
    detail.innerHTML =
        `<h6 style="color:${REGIME_COLORS[regime] || '#333'}">${regime}</h6>
         <ul class="mb-0">
           ${species.map(s => {
            const extra = isMass
                ? `masse : ${(s.quantity * s.weight).toFixed(1)} kg`
                : `qté : ${s.quantity}`;
            return `<li><strong>${s.name}</strong> — ${s.type} — ${extra} — poids unitaire : ${s.weight} kg</li>`;
        }).join('')}
         </ul>`;
}

function initDoughnutChart(data) {
    _doughnutData = data;
    _buildDoughnutChart(data);
}

function _buildDoughnutChart(data) {
    const isMass = _doughnutMode === 'mass';

    const regimeMap = {};
    data.species.forEach(s => {
        const val = isMass ? s.quantity * s.weight : s.quantity;
        regimeMap[s.regime] = (regimeMap[s.regime] || 0) + val;
    });

    const labels = Object.keys(regimeMap);
    const values = Object.values(regimeMap);
    const colors = labels.map(r => REGIME_COLORS[r] || '#999');
    const total = values.reduce((a, b) => a + b, 0);

    // Légende interactive : cliquable même pour les segments invisibles
    const legendEl = document.getElementById('legend-doughnut');
    if (legendEl) {
        legendEl.innerHTML = labels.map((label, i) => {
            const val = values[i];
            const pct = formatPct(val, total);
            const formatted = isMass
                ? (val >= 1000 ? `${(val / 1000).toFixed(1)} t` : `${val.toFixed(0)} kg`)
                : `${val} ind.`;
            return `<button type="button"
                        class="d-inline-flex align-items-center gap-1 btn btn-sm p-1 pe-2 rounded-pill border-0"
                        style="background:${colors[i]}20;cursor:pointer;"
                        data-regime="${label}"
                        onclick="_showDoughnutDetail('${label.replace(/'/g, "\\'")}')">
                        <span style="width:12px;height:12px;background:${colors[i]};
                              border-radius:50%;flex-shrink:0;display:inline-block;"></span>
                        <small><strong>${label}</strong> — ${formatted} (${pct})</small>
                    </button>`;
        }).join('');
    }

    const canvas = document.getElementById('chart-doughnut');
    if (!canvas) return;
    _doughnutChart = new Chart(canvas.getContext('2d'), {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {display: false},
                tooltip: {
                    callbacks: {
                        label(item) {
                            const isMass = _doughnutMode === 'mass';
                            const total = _doughnutChart.data.datasets[0].data
                                .reduce((a, b) => a + b, 0);
                            const val = item.raw;
                            const formatted = isMass
                                ? (val >= 1000 ? `${(val / 1000).toFixed(1)} t` : `${val.toFixed(0)} kg`)
                                : `${val} individus`;
                            return ` ${formatted} (${formatPct(val, total)})`;
                        }
                    }
                }
            },
            onClick(_event, elements) {
                if (!elements.length) return;
                _showDoughnutDetail(labels[elements[0].index]);
            }
        }
    });
}

// ─── Radar : niveaux trophiques ─────────────────────────────────────────────

let _radarChart = null;

function initRadarChart(data) {
    const levelMap = {};
    data.species.forEach(s => {
        const label = s.level === 0 ? 'Apex (niv. 0)' : `Niveau ${s.level}`;
        levelMap[label] = (levelMap[label] || 0) + 1;
    });

    let labels = Object.keys(levelMap).sort();
    let values = labels.map(l => levelMap[l]);

    // Un radar exige au moins 3 axes
    while (labels.length < 3) {
        labels.push('—');
        values.push(0);
    }

    renderLegend('legend-radar',
        ["Nombre d'espèces par niveau trophique (Apex = niv. 0)"],
        ['#3266ad']);

    const ctx = document.getElementById('chart-radar').getContext('2d');
    _radarChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels,
            datasets: [{
                label: 'Espèces',
                data: values,
                backgroundColor: 'rgba(50,102,173,0.2)',
                borderColor: '#3266ad',
                pointBackgroundColor: '#3266ad',
                pointRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {legend: {display: false}},
            scales: {
                r: {
                    beginAtZero: true,
                    ticks: {stepSize: 1, precision: 0}
                }
            }
        }
    });
}

window.updateRadarChart = function (species) {
    if (_radarChart) { _radarChart.destroy(); _radarChart = null; }
    initRadarChart({ species });
};

// UPDATE

window.updateBarChart = function (data) {
    //if (!_barChart) return;

    _barData = { species: data };

    if (_barChart) { _barChart.destroy(); _barChart = null; }
    _buildBarChart(_barData);

    const isMass = _barMode === 'mass';

    const typeMap = {};
    data.forEach(s => {
        const val = isMass ? s.quantity * s.weight : s.quantity;
        typeMap[s.type] = (typeMap[s.type] || 0) + val;
    });

    const labels = Object.keys(typeMap);
    const values = Object.values(typeMap);

    _barChart.data.labels = labels;
    _barChart.data.datasets[0].data = values;
    _barChart.update();
};

window.updateDoughnutChart = function (data) {
    //if (!_doughnutChart) return;

    _doughnutData = { species: data };

    if (_doughnutChart) { _doughnutChart.destroy(); _doughnutChart = null; }
    _buildDoughnutChart(_doughnutData);

    const isMass = _doughnutMode === 'mass';

    const regimeMap = {};
    data.forEach(s => {
        const val = isMass ? s.quantity * s.weight : s.quantity;
        regimeMap[s.regime] = (regimeMap[s.regime] || 0) + val;
    });

    const labels = Object.keys(regimeMap);
    const values = Object.values(regimeMap);

    _doughnutChart.data.labels = labels;
    _doughnutChart.data.datasets[0].data = values;
    _doughnutChart.update();
};