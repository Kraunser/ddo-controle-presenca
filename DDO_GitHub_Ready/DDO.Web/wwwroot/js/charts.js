// Configurações globais do Chart.js
Chart.defaults.font.family = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";
Chart.defaults.color = '#6b7280';
Chart.defaults.borderColor = '#e5e7eb';

// Paleta de cores Randoncorp
const colors = {
    primary: '#3b82f6',
    success: '#10b981',
    warning: '#f59e0b',
    danger: '#ef4444',
    info: '#8b5cf6',
    secondary: '#6b7280'
};

const colorPalette = [
    '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6',
    '#06b6d4', '#84cc16', '#f97316', '#ec4899', '#64748b'
];

// Função para renderizar gráfico de linha
window.renderLineChart = (canvasId, labels, data, label) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.charts && window.charts[canvasId]) {
        window.charts[canvasId].destroy();
    }

    const chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                borderColor: colors.primary,
                backgroundColor: colors.primary + '20',
                borderWidth: 3,
                fill: true,
                tension: 0.4,
                pointBackgroundColor: colors.primary,
                pointBorderColor: '#ffffff',
                pointBorderWidth: 2,
                pointRadius: 6,
                pointHoverRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: '#1f2937',
                    titleColor: '#ffffff',
                    bodyColor: '#ffffff',
                    borderColor: colors.primary,
                    borderWidth: 1,
                    cornerRadius: 8,
                    displayColors: false
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: '#f3f4f6'
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                }
            },
            elements: {
                point: {
                    hoverBackgroundColor: colors.primary
                }
            }
        }
    });

    // Armazenar referência do gráfico
    if (!window.charts) window.charts = {};
    window.charts[canvasId] = chart;
};

// Função para renderizar gráfico de pizza
window.renderPieChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.charts && window.charts[canvasId]) {
        window.charts[canvasId].destroy();
    }

    const chart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colorPalette.slice(0, labels.length),
                borderColor: '#ffffff',
                borderWidth: 3,
                hoverBorderWidth: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 20,
                        usePointStyle: true,
                        pointStyle: 'circle',
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    backgroundColor: '#1f2937',
                    titleColor: '#ffffff',
                    bodyColor: '#ffffff',
                    borderColor: colors.primary,
                    borderWidth: 1,
                    cornerRadius: 8,
                    callbacks: {
                        label: function(context) {
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((context.parsed / total) * 100).toFixed(1);
                            return `${context.label}: ${context.parsed} (${percentage}%)`;
                        }
                    }
                }
            },
            cutout: '60%'
        }
    });

    // Armazenar referência do gráfico
    if (!window.charts) window.charts = {};
    window.charts[canvasId] = chart;
};

// Função para renderizar gráfico de barras
window.renderBarChart = (canvasId, labels, data, label) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.charts && window.charts[canvasId]) {
        window.charts[canvasId].destroy();
    }

    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: colors.primary + '80',
                borderColor: colors.primary,
                borderWidth: 2,
                borderRadius: 6,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: '#1f2937',
                    titleColor: '#ffffff',
                    bodyColor: '#ffffff',
                    borderColor: colors.primary,
                    borderWidth: 1,
                    cornerRadius: 8,
                    displayColors: false
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: '#f3f4f6'
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                }
            }
        }
    });

    // Armazenar referência do gráfico
    if (!window.charts) window.charts = {};
    window.charts[canvasId] = chart;
};

// Função para renderizar gráfico de barras múltiplas
window.renderMultiBarChart = (canvasId, labels, datasets) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.charts && window.charts[canvasId]) {
        window.charts[canvasId].destroy();
    }

    const chartDatasets = datasets.map((dataset, index) => ({
        label: dataset.label,
        data: dataset.data,
        backgroundColor: colorPalette[index] + '80',
        borderColor: colorPalette[index],
        borderWidth: 2,
        borderRadius: 6,
        borderSkipped: false
    }));

    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: chartDatasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        padding: 20,
                        usePointStyle: true,
                        pointStyle: 'circle'
                    }
                },
                tooltip: {
                    backgroundColor: '#1f2937',
                    titleColor: '#ffffff',
                    bodyColor: '#ffffff',
                    borderColor: colors.primary,
                    borderWidth: 1,
                    cornerRadius: 8
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: '#f3f4f6'
                    },
                    ticks: {
                        color: '#6b7280'
                    }
                }
            }
        }
    });

    // Armazenar referência do gráfico
    if (!window.charts) window.charts = {};
    window.charts[canvasId] = chart;
};

// Função para atualizar dados de um gráfico existente
window.updateChart = (canvasId, newLabels, newData) => {
    if (window.charts && window.charts[canvasId]) {
        const chart = window.charts[canvasId];
        chart.data.labels = newLabels;
        chart.data.datasets[0].data = newData;
        chart.update('active');
    }
};

// Função para redimensionar todos os gráficos
window.resizeCharts = () => {
    if (window.charts) {
        Object.values(window.charts).forEach(chart => {
            chart.resize();
        });
    }
};

// Adicionar listener para redimensionamento da janela
window.addEventListener('resize', () => {
    setTimeout(window.resizeCharts, 100);
});

// Função auxiliar para scroll automático
window.scrollToTop = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = 0;
    }
};

// Função para mostrar alertas
window.showAlert = (type, title, message) => {
    // Implementação básica - pode ser substituída por uma biblioteca de toast/alert
    const alertClass = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    }[type] || 'alert-info';

    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            <strong>${title}</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    document.body.insertAdjacentHTML('beforeend', alertHtml);

    // Auto-remover após 5 segundos
    setTimeout(() => {
        const alerts = document.querySelectorAll('.alert');
        if (alerts.length > 0) {
            alerts[alerts.length - 1].remove();
        }
    }, 5000);
};

// Função para download de arquivo via JavaScript
window.downloadFile = (url) => {
    const link = document.createElement('a');
    link.href = url;
    link.download = '';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// Função para download de arquivo a partir de base64
window.downloadFileFromBase64 = (base64Data, fileName, mimeType) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
};
