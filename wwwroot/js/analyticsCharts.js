// added this file for Chart.js interop helper used by the Analytics page
// this is called from Blazor using IJSRuntime

(function () {
    // added this to keep chart instances so we can destroy and re-render safely
    const state = {
        entriesChart: null,
        moodChart: null
    }

    function getCtx(id) {
        const canvas = document.getElementById(id)
        if (!canvas) return null
        return canvas.getContext('2d')
    }

    function destroyChart(chart) {
        try {
            if (chart) chart.destroy()
        } catch {
            // keeping this empty so chart destroy errors don't crash the UI
        }
    }

    window.analyticsCharts = {
        render: function (entryLabels, entryCounts, moodLabels, moodValues) {
            // added this to prevent crashes if analytics data comes in as null
            entryLabels = entryLabels || []
            entryCounts = entryCounts || []
            moodLabels = moodLabels || []
            moodValues = moodValues || []

            // added this to prevent double-render + memory leaks
            destroyChart(state.entriesChart)
            destroyChart(state.moodChart)
            state.entriesChart = null
            state.moodChart = null

            // entries per day (line chart)
            const ectx = getCtx('entriesChart')
            if (ectx && window.Chart) {
                state.entriesChart = new Chart(ectx, {
                    type: 'line',
                    data: {
                        labels: entryLabels,
                        datasets: [{
                            label: 'Entries',
                            data: entryCounts,
                            tension: 0.3
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { display: false },
                            tooltip: { enabled: true }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: { precision: 0 }
                            }
                        }
                    }
                })
            }

            // mood distribution (doughnut chart)
            const mctx = getCtx('moodChart')
            if (mctx && window.Chart) {
                state.moodChart = new Chart(mctx, {
                    type: 'doughnut',
                    data: {
                        labels: moodLabels,
                        datasets: [{
                            label: 'Moods',
                            data: moodValues
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                display: true,
                                position: 'bottom'
                            }
                        }
                    }
                })
            }
        },

        destroy: function () {
            // added this so we can clean charts when leaving the page
            destroyChart(state.entriesChart)
            destroyChart(state.moodChart)
            state.entriesChart = null
            state.moodChart = null
        }
    }
})()
