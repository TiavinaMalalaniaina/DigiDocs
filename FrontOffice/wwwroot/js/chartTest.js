$(function() {
    // Supposons que vous ayez déjà des données à afficher, par exemple :
    const data = {
        labels: ['Janvier', 'Fevrier', 'Mars', 'Avril', 'Mai', 'Juin', 'Juillet', 'Aout', 'Septembre', 'Octobre', 'Novembre', 'Desembre'],
        datasets: [{
            label: 'Sales',
            data: JSON.parse($('#barChart').attr("data-data")),
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgba(255, 99, 132, 1)',
            borderWidth: 1
        }]
    };

    // Sélectionnez le canvas du graphique en barres par son ID
    const barChartCanvas = document.getElementById('barChart');

    // Initialisez le graphique en barres avec les données
    const barChart = new Chart(barChartCanvas, {
        type: 'bar',
        data: data
    });

});