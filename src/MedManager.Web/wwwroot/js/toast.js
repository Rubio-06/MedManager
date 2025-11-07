// Toast Notification System
(function () {
  "use strict";

  // Créer le conteneur de toasts s'il n'existe pas
  function getToastContainer() {
    let container = document.getElementById("toast-container");
    if (!container) {
      container = document.createElement("div");
      container.id = "toast-container";
      container.className =
        "fixed bottom-4 right-4 z-50 flex flex-col-reverse gap-3 pointer-events-none";
      container.style.maxWidth = "420px";
      document.body.appendChild(container);
    }
    return container;
  }

  // Types de toast avec leurs configurations
  const toastTypes = {
    success: {
      bgColor: "bg-green-50",
      borderColor: "border-green-500",
      textColor: "text-green-900",
      iconColor: "text-green-500",
      icon: `<svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
            </svg>`,
    },
    error: {
      bgColor: "bg-red-50",
      borderColor: "border-red-500",
      textColor: "text-red-900",
      iconColor: "text-red-500",
      icon: `<svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
            </svg>`,
    },
    warning: {
      bgColor: "bg-yellow-50",
      borderColor: "border-yellow-500",
      textColor: "text-yellow-900",
      iconColor: "text-yellow-600",
      icon: `<svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
            </svg>`,
    },
    info: {
      bgColor: "bg-indigo-50",
      borderColor: "border-indigo-500",
      textColor: "text-indigo-900",
      iconColor: "text-indigo-600",
      icon: `<svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
            </svg>`,
    },
  };

  // Fonction principale pour afficher un toast
  window.showToast = function (message, type = "info", duration = 4000) {
    const container = getToastContainer();
    const config = toastTypes[type] || toastTypes.info;

    // Créer l'élément toast
    const toast = document.createElement("div");
    toast.className = `${config.bgColor} ${config.borderColor} border-l-4 p-4 rounded-lg shadow-lg pointer-events-auto transform transition-all duration-300 ease-in-out opacity-0 translate-x-full`;

    toast.innerHTML = `
            <div class="flex items-start">
                <div class="shrink-0 ${config.iconColor}">
                    ${config.icon}
                </div>
                <div class="ml-3 flex-1">
                    <p class="text-sm font-medium ${config.textColor}">
                        ${message}
                    </p>
                </div>
                <button type="button" class="ml-4 shrink-0 inline-flex ${config.textColor} hover:${config.textColor} focus:outline-none transition-opacity duration-150 opacity-70 hover:opacity-100" onclick="this.parentElement.parentElement.remove()">
                    <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
                    </svg>
                </button>
            </div>
        `;

    // Ajouter au conteneur
    container.appendChild(toast);

    // Animation d'entrée
    setTimeout(() => {
      toast.classList.remove("opacity-0", "translate-x-full");
    }, 10);

    // Auto-suppression après la durée spécifiée
    if (duration > 0) {
      setTimeout(() => {
        removeToast(toast);
      }, duration);
    }

    return toast;
  };

  // Fonction pour retirer un toast avec animation
  function removeToast(toast) {
    toast.classList.add("opacity-0", "translate-x-full");
    setTimeout(() => {
      if (toast.parentElement) {
        toast.remove();
      }
    }, 300);
  }

  // Fonctions raccourcies pour chaque type
  window.showSuccessToast = function (message, duration = 4000) {
    return showToast(message, "success", duration);
  };

  window.showErrorToast = function (message, duration = 5000) {
    return showToast(message, "error", duration);
  };

  window.showWarningToast = function (message, duration = 4000) {
    return showToast(message, "warning", duration);
  };

  window.showInfoToast = function (message, duration = 4000) {
    return showToast(message, "info", duration);
  };

  // Convertir les messages TempData en toasts au chargement de la page
  document.addEventListener("DOMContentLoaded", function () {
    // Chercher les messages de succès
    const successMessages = document.querySelectorAll("[data-toast-success]");
    successMessages.forEach((el) => {
      showSuccessToast(el.getAttribute("data-toast-success"));
      el.remove();
    });

    // Chercher les messages d'erreur
    const errorMessages = document.querySelectorAll("[data-toast-error]");
    errorMessages.forEach((el) => {
      showErrorToast(el.getAttribute("data-toast-error"));
      el.remove();
    });

    // Chercher les messages d'avertissement
    const warningMessages = document.querySelectorAll("[data-toast-warning]");
    warningMessages.forEach((el) => {
      showWarningToast(el.getAttribute("data-toast-warning"));
      el.remove();
    });

    // Chercher les messages d'info
    const infoMessages = document.querySelectorAll("[data-toast-info]");
    infoMessages.forEach((el) => {
      showInfoToast(el.getAttribute("data-toast-info"));
      el.remove();
    });
  });
})();
