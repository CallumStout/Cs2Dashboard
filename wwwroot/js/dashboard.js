async function loadStats() {
    const response = await fetch("http://localhost:5050/stats");
    const data = await response.json();

    const tbody = document.querySelector("#statsTable tbody");
    tbody.innerHTML = "";

    data.forEach(p => {
        tbody.innerHTML += `
                <tr>
                    <td>${p.name}</td>
                    <td>${p.kills}</td>
                    <td>${p.deaths}</td>
                    <td>${p.assists}</td>
                    <td>${p.adr}</td>
                    <td>${new Date(p.lastUpdated).toLocaleTimeString()}</td>
                </tr>
            `;
    });
}

// Auto-refresh every 3 seconds
setInterval(loadStats, 3000);
loadStats();