'use strict';

const NET_REGIME_COLORS = {
    'Photosynthèse': '#2e9e6b',
    'Herbivore': '#3266ad',
    'Carnivore': '#e07b39',
    'Omnivore': '#f0b429',
    'Coprophages': '#8e6bbf',
    'Limivores': '#8e6bbf',
    'Detritivores': '#8e6bbf',
    'Heterotrophie': '#8e6bbf'
};

// ─── Point d'entrée ────────────────────────────────────────────────────────

function initBiomeNetwork(data, options = {}) {
    const mode = options.mode ?? 'view';
    const selected = options.selectedSpecies;

    let predators = new Set();
    let prey = new Set();
    
    const container = document.getElementById('network-container');
    if (!container) return;

    if (!data.species || data.species.length === 0) {
        container.innerHTML =
            '<p class="text-muted text-center py-5">Aucune espèce dans ce biome.</p>';
        return;
    }

    const width = container.clientWidth || 800;
    const height = container.clientHeight || 500;

    // Légende régimes présents
    const regimes = [...new Set(data.species.map(s => s.regime))];
    const legendEl = document.getElementById('legend-network');
    if (legendEl) {
        legendEl.innerHTML = regimes.map(r =>
            `<span class="d-inline-flex align-items-center gap-1">
                <span style="width:13px;height:13px;background:${NET_REGIME_COLORS[r] || '#999'};
                      border-radius:50%;flex-shrink:0;display:inline-block;"></span>
                <small>${r}</small>
             </span>`
        ).join('');
    }

    const svg = d3.select('#network-container')
        .append('svg')
        .attr('width', width)
        .attr('height', height);

    // Flèche (prédateur → proie)
    svg.append('defs').append('marker')
        .attr('id', 'bd-arrow')
        .attr('viewBox', '0 -5 10 10')
        .attr('refX', 10)
        .attr('refY', 0)
        .attr('markerWidth', 7)
        .attr('markerHeight', 7)
        .attr('orient', 'auto')
        .append('path')
        .attr('d', 'M0,-5L10,0L0,5')
        .attr('fill', '#aaa');

    const nodes = data.species.map(s => ({...s}));
    const links = data.predations.map(p => ({source: p.predator, target: p.prey}));

    if (mode === 'edit' && selected) {

        predators = new Set(
            data.predations
                .filter(p => p.prey === selected)
                .map(p => p.predator)
        );

        prey = new Set(
            data.predations
                .filter(p => p.predator === selected)
                .map(p => p.prey)
        );
    }

    const nodeR = d => Math.min(Math.sqrt(d.quantity) * 4 + 10, 36);

    const linkStrength = links.length === 0 ? 0 : undefined;

    const simulation = d3.forceSimulation(nodes)
        .force('link', d3.forceLink(links).id(d => d.name).distance(130).strength(linkStrength ?? 1))
        .force('charge', d3.forceManyBody().strength(links.length === 0 ? -80 : -380))
        .force('center', d3.forceCenter(width / 2, height / 2))
        .force('x', d3.forceX(width / 2).strength(0.08))
        .force('y', d3.forceY(height / 2).strength(0.08))
        .force('collision', d3.forceCollide().radius(d => nodeR(d) + 12));

    const linkSel = svg.append('g')
        .selectAll('line')
        .data(links)
        .join('line')
        .attr('stroke', '#ccc')
        .attr('stroke-width', 1.8)
        .attr('marker-end', 'url(#bd-arrow)');

    const nodeSel = svg.append('g')
        .selectAll('g')
        .data(nodes)
        .join('g')
        .style('cursor', 'pointer')
        .call(d3.drag()
            .on('start', dragStart)
            .on('drag', dragged)
            .on('end', dragEnd))
        .on('click', showDetail);

    nodeSel.append('circle')
        .attr('r', nodeR)
        .attr('fill', d => {

            // MODE DASHBOARD = inchangé
            if (mode !== 'edit') {
                return NET_REGIME_COLORS[d.regime] || '#999';
            }

            // MODE EDIT = coloration relationnelle
            if (d.name === selected) return '#0d6efd';       // bleu (cible)

            if (predators.has(d.name)) return '#dc3545';     // rouge (prédateurs)

            if (prey.has(d.name)) return '#198754';          // vert (proies)

            return '#adb5bd';                                // neutre
        })
        .attr('stroke', '#fff')
        .attr('stroke-width', 2.5);

    nodeSel.append('text')
        .text(d => d.name.length > 10 ? d.name.slice(0, 9) + '…' : d.name)
        .attr('text-anchor', 'middle')
        .attr('dy', d => nodeR(d) + 14)
        .attr('font-size', '11px')
        .attr('fill', '#333')
        .style('pointer-events', 'none');

    nodeSel.append('title')
        .text(d =>
            `${d.name}\nType : ${d.type}\nRégime : ${d.regime}\nQté : ${d.quantity}\nPoids : ${d.weight} kg`
        );

    simulation.on('tick', () => {
        nodes.forEach(d => {
            const r = nodeR(d);
            d.x = Math.max(r, Math.min(width - r, d.x));
            d.y = Math.max(r, Math.min(height - r, d.y));
        });

        linkSel.each(function (d) {
            const dx = d.target.x - d.source.x;
            const dy = d.target.y - d.source.y;
            const dist = Math.sqrt(dx * dx + dy * dy) || 1;
            const srcR = nodeR(d.source);
            const tgtR = nodeR(d.target);
            d3.select(this)
                .attr('x1', d.source.x + dx / dist * srcR)
                .attr('y1', d.source.y + dy / dist * srcR)
                .attr('x2', d.target.x - dx / dist * tgtR)
                .attr('y2', d.target.y - dy / dist * tgtR);
        });

        nodeSel.attr('transform', d => `translate(${d.x},${d.y})`);
    });

    // ── Drag ───────────────────────────────────────────────────────────────

    function dragStart(event, d) {
        if (!event.active) simulation.alphaTarget(0.3).restart();
        d.fx = d.x;
        d.fy = d.y;
    }

    function dragged(event, d) {
        d.fx = event.x;
        d.fy = event.y;
    }

    function dragEnd(event, d) {
        if (!event.active) simulation.alphaTarget(0);
        d.fx = null;
        d.fy = null;
    }

    // ── Panneau de détail au clic ──────────────────────────────────────────

    function showDetail(event, d) {
        const predators = data.predations.filter(p => p.prey === d.name).map(p => p.predator);
        const prey = data.predations.filter(p => p.predator === d.name).map(p => p.prey);
        const detail = document.getElementById('detail-network');
        detail.classList.remove('d-none');
        detail.innerHTML =
            `<strong>${d.name}</strong> (${d.type})
             &nbsp;·&nbsp; Régime : ${d.regime}
             &nbsp;·&nbsp; Quantité : ${d.quantity}
             &nbsp;·&nbsp; Poids moy. : ${d.weight} kg
             ${predators.length ? `<br>🔴 Prédateurs : ${predators.join(', ')}` : ''}
             ${prey.length ? `<br>🟢 Proies : ${prey.join(', ')}` : ''}`;
    }
}

// ─── Export réseau en PNG ──────────────────────────────────────────────────

function exportNetwork() {
    const svgEl = document.querySelector('#network-container svg');
    if (!svgEl) return;

    const w = svgEl.clientWidth || 800;
    const h = svgEl.clientHeight || 500;

    const serializer = new XMLSerializer();
    const svgStr = serializer.serializeToString(svgEl);
    const blob = new Blob([svgStr], {type: 'image/svg+xml;charset=utf-8'});
    const url = URL.createObjectURL(blob);

    const img = new Image();
    img.onload = function () {
        const canvas = document.createElement('canvas');
        canvas.width = w;
        canvas.height = h;
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = '#fafafa';
        ctx.fillRect(0, 0, w, h);
        ctx.drawImage(img, 0, 0);
        URL.revokeObjectURL(url);
        const link = document.createElement('a');
        link.download = 'reseau-trophique.png';
        link.href = canvas.toDataURL('image/png');
        link.click();
    };
    img.src = url;
}

// UPDATE

window.updateNetwork = function (predations) {
    const container = document.getElementById('network-container');
    container.innerHTML = '';

    // rebuild complet (D3 = plus simple que patch)
    initBiomeNetwork(window.BIOME_DATA);
};