// Note Modal Functionality
// Shared functions for opening and managing note modals across different report pages
// Note: Variables currentAccountName, currentNoteId, and accountNotes should be declared in each page

function openNoteModalDefault(accountName) {
    // Fallback function if the page doesn't have its own implementation
    console.warn('openNoteModal called but not implemented on this page.');
    alert('Note functionality is not available on this page.');
}

function viewNoteModal(accountName) {
    // Same as openNoteModal for non-treasurers
    openNoteModal(accountName);
}

function closeNoteModal() {
    const modal = document.getElementById('noteModal');
    const modalNoteStatus = document.getElementById('modalNoteStatus');
    
    if (modal) {
        modal.classList.add('hidden');
    }
    if (modalNoteStatus) {
        modalNoteStatus.classList.add('hidden');
    }
}

function saveAccountNote() {
    const isTreasurer = window.isTreasurer || false;
    if (!isTreasurer) return; // Extra safety check
    
    const modalNoteContent = document.getElementById('modalNoteContent');
    const statusDiv = document.getElementById('modalNoteStatus');
    
    if (!modalNoteContent || !statusDiv) {
        console.error('Required modal elements not found');
        return;
    }
    
    const content = modalNoteContent.value;
    
    // Show saving status
    statusDiv.className = 'text-sm text-blue-600';
    statusDiv.textContent = 'Saving...';
    statusDiv.classList.remove('hidden');
    
    // Get form data
    const formData = new FormData();
    formData.append('reportType', window.currentReportType || 'General');
    formData.append('content', content);
    formData.append('fromDate', window.reportFromDate || '');
    formData.append('toDate', window.reportToDate || '');
    formData.append('noteId', currentNoteId);
    formData.append('accountName', currentAccountName);
    
    const token = document.querySelector('input[name="__RequestVerificationToken"]');
    if (token) {
        formData.append('__RequestVerificationToken', token.value);
    }
    
    fetch('/Reports/SaveNote', {
        method: 'POST',
        body: formData,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            statusDiv.className = 'text-sm text-green-600';
            statusDiv.textContent = 'Note saved successfully!';
            
            // Update local cache
            accountNotes[currentAccountName] = {
                noteId: data.noteId || currentNoteId,
                content: content
            };
            
            // Update the note icon to show it has content
            updateNoteIcon(currentAccountName, content.trim() !== '');
            
            setTimeout(() => {
                closeNoteModal();
            }, 1500);
        } else {
            statusDiv.className = 'text-sm text-red-600';
            statusDiv.textContent = 'Error: ' + (data.message || 'Unknown error');
        }
    })
    .catch(error => {
        console.error('Error saving note:', error);
        statusDiv.className = 'text-sm text-red-600';
        statusDiv.textContent = 'Error saving note. Please try again.';
    });
}

function updateNoteIcon(accountName, hasContent) {
    // Find the note button for this account and update its appearance
    const buttons = document.querySelectorAll(`button[onclick*="${accountName}"]`);
    buttons.forEach(button => {
        if (hasContent) {
            button.classList.add('bg-cascadeBlue', 'text-white');
            button.classList.remove('bg-slate-100');
        } else {
            button.classList.add('bg-slate-100');
            button.classList.remove('bg-cascadeBlue', 'text-white');
        }
    });
}

// Initialize modal functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    const modal = document.getElementById('noteModal');
    
    if (modal) {
        // Close modal on Escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                closeNoteModal();
            }
        });
        
        // Close modal when clicking outside
        modal.addEventListener('click', function(e) {
            if (e.target === this) {
                closeNoteModal();
            }
        });
    }
});
