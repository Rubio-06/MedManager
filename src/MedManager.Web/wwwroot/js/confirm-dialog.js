// Utilitaire de dialogue de confirmation
window.ConfirmDialog = {
  show(options) {
    const defaults = {
      title: "Confirmation",
      message: "Êtes-vous sûr de vouloir continuer ?",
      confirmText: "Confirmer",
      cancelText: "Annuler",
      confirmClass: "bg-red-600 hover:bg-red-700",
      onConfirm: () => {},
      onCancel: () => {},
    };

    const config = { ...defaults, ...options };

    // Créer le dialogue
    const dialog = document.createElement("div");
    dialog.id = "confirm-dialog";
    dialog.className = "fixed inset-0 z-50 flex items-center justify-center";
    dialog.innerHTML = `
            <!-- Overlay -->
            <div class="fixed inset-0 bg-black/50 bg-opacity-50 transition-opacity" id="dialog-overlay"></div>
            
            <!-- Dialog -->
            <div class="relative bg-white rounded-lg shadow-xl max-w-md w-full mx-4 transform transition-all" id="dialog-content">
                <!-- Header -->
                <div class="px-6 py-4 border-b border-gray-200">
                    <div class="flex items-center">
                        <div class="flex-shrink-0 w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
                            <svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                            </svg>
                        </div>
                        <h3 class="ml-3 text-lg font-semibold text-gray-900" id="dialog-title">${config.title}</h3>
                    </div>
                </div>

                <!-- Body -->
                <div class="px-6 py-4">
                    <p class="text-gray-600" id="dialog-message">${config.message}</p>
                </div>

                <!-- Footer -->
                <div class="px-6 py-4 bg-gray-50 rounded-b-lg flex gap-3 justify-end">
                    <button type="button" id="dialog-cancel" class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors font-medium">
                        ${config.cancelText}
                    </button>
                    <button type="button" id="dialog-confirm" class="px-4 py-2 text-white rounded-lg transition-colors font-medium ${config.confirmClass}">
                        ${config.confirmText}
                    </button>
                </div>
            </div>
        `;

    document.body.appendChild(dialog);

    // Animation d'entrée
    requestAnimationFrame(() => {
      dialog.querySelector("#dialog-overlay").style.opacity = "1";
      dialog.querySelector("#dialog-content").style.transform = "scale(1)";
    });

    // Gestionnaires d'événements
    const confirmBtn = dialog.querySelector("#dialog-confirm");
    const cancelBtn = dialog.querySelector("#dialog-cancel");
    const overlay = dialog.querySelector("#dialog-overlay");

    const close = (confirmed) => {
      // Animation de sortie
      dialog.querySelector("#dialog-overlay").style.opacity = "0";
      dialog.querySelector("#dialog-content").style.transform = "scale(0.95)";

      setTimeout(() => {
        dialog.remove();
        if (confirmed) {
          config.onConfirm();
        } else {
          config.onCancel();
        }
      }, 200);
    };

    confirmBtn.addEventListener("click", () => close(true));
    cancelBtn.addEventListener("click", () => close(false));
    overlay.addEventListener("click", () => close(false));

    // ESC pour fermer
    const escHandler = (e) => {
      if (e.key === "Escape") {
        close(false);
        document.removeEventListener("keydown", escHandler);
      }
    };
    document.addEventListener("keydown", escHandler);

    // Focus sur le bouton annuler par défaut
    cancelBtn.focus();
  },
};

// Helper pour les formulaires de suppression
window.confirmDelete = function (form, options = {}) {
  const defaults = {
    title: "Confirmer la suppression",
    message: "Cette action est irréversible. Êtes-vous sûr de vouloir supprimer cet élément ?",
    confirmText: "Supprimer",
    cancelText: "Annuler",
    confirmClass: "bg-red-600 hover:bg-red-700",
  };

  const config = { ...defaults, ...options };

  ConfirmDialog.show({
    ...config,
    onConfirm: () => form.submit(),
  });

  return false; // Empêche la soumission immédiate du formulaire
};

// Helper générique pour toute action dangereuse
window.confirmAction = function (callback, options = {}) {
  const defaults = {
    title: "Confirmation requise",
    message: "Êtes-vous sûr de vouloir effectuer cette action ?",
    confirmText: "Confirmer",
    cancelText: "Annuler",
    confirmClass: "bg-blue-600 hover:bg-blue-700",
  };

  const config = { ...defaults, ...options };

  ConfirmDialog.show({
    ...config,
    onConfirm: callback,
  });

  return false;
};

// Helper simplifié pour compatibilité avec les vues
window.showConfirmDialog = function (
  title,
  message,
  confirmText,
  cancelText,
  onConfirm,
  onCancel = () => {}
) {
  ConfirmDialog.show({
    title: title,
    message: message,
    confirmText: confirmText,
    cancelText: cancelText,
    confirmClass: "bg-red-600 hover:bg-red-700",
    onConfirm: onConfirm,
    onCancel: onCancel,
  });
};
