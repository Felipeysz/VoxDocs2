        async function selectPastaPrincipal(nomePastaPrincipal) {
            // Atualiza a barra de progresso
            document.querySelector('.progress').style.width = '66%';
            document.querySelectorAll('.steps .step')[1].classList.add('active');

            // Oculta a etapa 1 e mostra a etapa 2
            document.getElementById('etapa1').classList.add('animate__fadeOutLeft');
            setTimeout(() => {
                document.getElementById('etapa1').classList.add('d-none');
                document.getElementById('etapa2').classList.remove('d-none');
                document.getElementById('etapa2').classList.add('animate__fadeInRight');
            }, 300);

            // Busca as subpastas via API
            const response = await fetch(`/api/SubPasta/subchildren/${nomePastaPrincipal}`);
            const subPastas = await response.json();

            // Atualiza a lista de subpastas
            const subpastasList = document.getElementById('subpastas-list');
            subpastasList.innerHTML = subPastas.map(subPasta => `
                <li class="list-group-item">
                    <button onclick="selectSubPasta(${subPasta.Id})">
                        <span class="material-symbols-outlined">folder</span> ${subPasta.nomeSubPasta}
                    </button>
                </li>
            `).join('');
        }

        function selectSubPasta(subPastaId) {
            // Atualiza a barra de progresso
            document.querySelector('.progress').style.width = '100%';
            document.querySelectorAll('.steps .step')[2].classList.add('active');

            // Oculta a etapa 2 e mostra a etapa 3
            document.getElementById('etapa2').classList.add('animate__fadeOutLeft');
            setTimeout(() => {
                document.getElementById('etapa2').classList.add('d-none');
                document.getElementById('etapa3').classList.remove('d-none');
                document.getElementById('etapa3').classList.add('animate__fadeInRight');
            }, 300);
        }