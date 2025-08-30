// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Script para melhorar a experiência do usuário
// Script para melhorar a experiência do usuário
//document.addEventListener('DOMContentLoaded', function () {
//    // Auto-focus no campo de título quando a página carrega
//    const titleInput = document.querySelector('input[name="NewTodo.Title"]');
//    if (titleInput) {
//        titleInput.focus();
//    }

//    // Adicionar animação aos botões
//    const buttons = document.querySelectorAll('button[type="submit"]');
//    buttons.forEach(button => {
//        button.addEventListener('click', function () {
//            if (this.innerHTML.indexOf('spinner') === -1) {
//                const originalText = this.innerHTML;
//                this.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';
//                this.disabled = true;

//                // Restaurar texto original após 2 segundos (fallback)
//                setTimeout(() => {
//                    this.innerHTML = originalText;
//                    this.disabled = false;
//                }, 2000);
//            }
//        });
//    });
//});

document.addEventListener('DOMContentLoaded', function () {
    // Auto-focus no campo de título quando a página carrega
    const titleInput = document.querySelector('input[name="NewTodo.Title"]');
    if (titleInput) {
        titleInput.focus();
    }

    // Adicionar animação aos botões SEM impedir o submit
    const buttons = document.querySelectorAll('button[type="submit"]');
    buttons.forEach(button => {
        button.addEventListener('click', function () {
            if (this.innerHTML.indexOf('spinner') === -1) {
                const originalText = this.innerHTML;
                this.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> ' + originalText;

                // Remover o spinner após o processamento (você pode ajustar isso conforme necessário)
                setTimeout(() => {
                    this.innerHTML = originalText;
                }, 2000);
            }
        });
    });
});