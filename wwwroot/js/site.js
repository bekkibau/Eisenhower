// Eisenhower Matrix - Site JavaScript

(function () {
    'use strict';

    // ===== Theme Management =====
    const ThemeManager = {
        storageKey: 'theme',
        
        init() {
            // Load saved theme or default to light
            const savedTheme = localStorage.getItem(this.storageKey) || 'light';
            this.setTheme(savedTheme);
            
            // Set up theme toggle button
            const themeToggle = document.getElementById('themeToggle');
            if (themeToggle) {
                themeToggle.addEventListener('click', () => this.toggleTheme());
            }
            
            // Listen for system theme changes
            if (window.matchMedia) {
                window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
                    const currentTheme = localStorage.getItem(this.storageKey);
                    if (currentTheme === 'system') {
                        this.applySystemTheme();
                    }
                });
            }
        },
        
        setTheme(theme) {
            localStorage.setItem(this.storageKey, theme);
            
            if (theme === 'system') {
                this.applySystemTheme();
            } else {
                document.documentElement.setAttribute('data-bs-theme', theme);
            }
            
            this.updateToggleIcon();
        },
        
        applySystemTheme() {
            const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
            document.documentElement.setAttribute('data-bs-theme', prefersDark ? 'dark' : 'light');
        },
        
        toggleTheme() {
            const currentTheme = document.documentElement.getAttribute('data-bs-theme');
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            this.setTheme(newTheme);
            
            // Also update server-side setting
            fetch('/Settings/SetThemeAjax', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ theme: newTheme === 'dark' ? 1 : 0 })
            }).catch(err => console.log('Theme sync error:', err));
        },
        
        updateToggleIcon() {
            const themeToggle = document.getElementById('themeToggle');
            if (themeToggle) {
                const currentTheme = document.documentElement.getAttribute('data-bs-theme');
                const icon = themeToggle.querySelector('i');
                if (icon) {
                    icon.className = currentTheme === 'dark' ? 'bi bi-sun' : 'bi bi-moon-stars';
                }
            }
        }
    };

    // ===== Auto-dismiss alerts =====
    function initAlerts() {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            setTimeout(() => {
                const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
                bsAlert.close();
            }, 5000);
        });
    }

    // ===== Drag and Drop =====
    const DragDropManager = {
        draggedElement: null,
        draggedTaskId: null,

        init() {
            // Set up draggable tasks
            document.querySelectorAll('.task-card[draggable="true"]').forEach(card => {
                card.addEventListener('dragstart', (e) => this.handleDragStart(e));
                card.addEventListener('dragend', (e) => this.handleDragEnd(e));
            });

            // Set up drop zones (quadrants)
            document.querySelectorAll('.quadrant').forEach(quadrant => {
                quadrant.addEventListener('dragover', (e) => this.handleDragOver(e));
                quadrant.addEventListener('dragenter', (e) => this.handleDragEnter(e));
                quadrant.addEventListener('dragleave', (e) => this.handleDragLeave(e));
                quadrant.addEventListener('drop', (e) => this.handleDrop(e));
            });

            // Also allow dropping on task lists directly
            document.querySelectorAll('.task-list').forEach(list => {
                list.addEventListener('dragover', (e) => this.handleDragOver(e));
                list.addEventListener('drop', (e) => this.handleDrop(e));
            });
        },

        handleDragStart(e) {
            this.draggedElement = e.target.closest('.task-card');
            this.draggedTaskId = this.draggedElement.dataset.taskId;
            this.dropHandled = false;
            
            // Add dragging class
            this.draggedElement.classList.add('dragging');
            
            // Set drag data
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('text/plain', this.draggedTaskId);
            
            // Highlight all quadrants as potential drop zones
            document.querySelectorAll('.quadrant').forEach(q => {
                q.style.transition = 'all 0.2s ease';
            });
        },

        handleDragEnd(e) {
            // Only clean up visuals if drop wasn't handled
            if (!this.dropHandled && this.draggedElement) {
                this.draggedElement.classList.remove('dragging');
            }
            
            // Remove all drag-over styles
            document.querySelectorAll('.quadrant').forEach(q => {
                q.classList.remove('drag-over');
            });
            document.querySelectorAll('.task-list').forEach(l => {
                l.classList.remove('drag-over');
            });
            document.querySelectorAll('.drop-zone-empty').forEach(e => {
                e.classList.remove('drag-over');
            });
            
            // Delay clearing references to allow drop handler to complete
            setTimeout(() => {
                this.draggedElement = null;
                this.draggedTaskId = null;
                this.dropHandled = false;
            }, 100);
        },

        handleDragOver(e) {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'move';
        },

        handleDragEnter(e) {
            e.preventDefault();
            const quadrant = e.target.closest('.quadrant');
            if (quadrant) {
                quadrant.classList.add('drag-over');
            }
            const emptyZone = e.target.closest('.drop-zone-empty');
            if (emptyZone) {
                emptyZone.classList.add('drag-over');
            }
        },

        handleDragLeave(e) {
            const quadrant = e.target.closest('.quadrant');
            if (quadrant && !quadrant.contains(e.relatedTarget)) {
                quadrant.classList.remove('drag-over');
            }
            const emptyZone = e.target.closest('.drop-zone-empty');
            if (emptyZone && !emptyZone.contains(e.relatedTarget)) {
                emptyZone.classList.remove('drag-over');
            }
        },

        async handleDrop(e) {
            e.preventDefault();
            e.stopPropagation();
            
            const quadrant = e.target.closest('.quadrant');
            if (!quadrant || !this.draggedTaskId) return;
            
            // Mark drop as handled
            this.dropHandled = true;
            
            // Store references before async operation
            const taskId = this.draggedTaskId;
            const elementToMove = this.draggedElement;
            const sourceTaskList = elementToMove ? elementToMove.closest('.task-list') : null;
            
            // Get quadrant number from class
            let quadrantNumber = null;
            if (quadrant.classList.contains('quadrant-1')) quadrantNumber = 0;
            else if (quadrant.classList.contains('quadrant-2')) quadrantNumber = 1;
            else if (quadrant.classList.contains('quadrant-3')) quadrantNumber = 2;
            else if (quadrant.classList.contains('quadrant-4')) quadrantNumber = 3;
            
            if (quadrantNumber === null) return;
            
            // Remove drag-over styles
            quadrant.classList.remove('drag-over');
            document.querySelectorAll('.drag-over').forEach(el => el.classList.remove('drag-over'));
            
            // Send update to server
            try {
                const response = await fetch('/Tasks/MoveToQuadrant', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        taskId: parseInt(taskId),
                        quadrant: quadrantNumber
                    })
                });
                
                const result = await response.json();
                
                if (result.success) {
                    // Move the element visually
                    const targetTaskList = quadrant.querySelector('.task-list');
                    if (targetTaskList && elementToMove) {
                        // Remove empty state from target if present
                        const targetEmptyState = targetTaskList.querySelector('.empty-state');
                        if (targetEmptyState) {
                            targetEmptyState.remove();
                        }
                        
                        // Remove dragging class and move element
                        elementToMove.classList.remove('dragging');
                        targetTaskList.insertBefore(elementToMove, targetTaskList.firstChild);
                        
                        // Add empty state to source if it's now empty
                        if (sourceTaskList && sourceTaskList.querySelectorAll('.task-card').length === 0) {
                            const emptyState = document.createElement('div');
                            emptyState.className = 'empty-state drop-zone-empty';
                            emptyState.innerHTML = `
                                <i class="bi bi-inbox text-muted"></i>
                                <p class="text-muted small mb-0">No tasks</p>
                                <p class="text-muted small mb-0">Drop here</p>
                            `;
                            sourceTaskList.appendChild(emptyState);
                        }
                        
                        // Show success toast
                        this.showToast('Task moved successfully!', 'success');
                    }
                } else {
                    this.showToast(result.message || 'Failed to move task', 'error');
                    // Reload to restore state
                    window.location.reload();
                }
            } catch (error) {
                console.error('Error moving task:', error);
                this.showToast('An error occurred while moving the task', 'error');
                window.location.reload();
            }
        },

        showToast(message, type = 'info') {
            // Create a simple toast notification
            const toast = document.createElement('div');
            toast.className = `alert alert-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'info'} position-fixed`;
            toast.style.cssText = 'bottom: 20px; right: 20px; z-index: 9999; min-width: 250px; animation: fadeIn 0.3s ease;';
            toast.innerHTML = `
                <i class="bi ${type === 'success' ? 'bi-check-circle' : 'bi-exclamation-triangle'} me-2"></i>
                ${message}
            `;
            document.body.appendChild(toast);
            
            // Auto-remove after 3 seconds
            setTimeout(() => {
                toast.style.opacity = '0';
                toast.style.transition = 'opacity 0.3s ease';
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        }
    };

    // ===== Task Actions (AJAX) =====
    const TaskActions = {
        init() {
            // Set up AJAX toggle for task completion
            document.querySelectorAll('.toggle-form').forEach(form => {
                form.addEventListener('submit', (e) => this.handleToggle(e, form));
            });
            
            // Set up AJAX delete
            document.querySelectorAll('.delete-form').forEach(form => {
                form.addEventListener('submit', (e) => this.handleDelete(e, form));
            });
        },
        
        async handleToggle(e, form) {
            // For now, let the form submit normally
            // This can be enhanced to use AJAX for smoother UX
        },
        
        async handleDelete(e, form) {
            // Form already has confirmation, let it submit normally
        }
    };

    // ===== Keyboard Shortcuts =====
    function initKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + N = New Task
            if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
                e.preventDefault();
                window.location.href = '/Tasks/Create';
            }
            
            // Escape = Go Home
            if (e.key === 'Escape' && !document.querySelector('.modal.show')) {
                const isOnForm = document.activeElement.tagName === 'INPUT' || 
                                 document.activeElement.tagName === 'TEXTAREA' ||
                                 document.activeElement.tagName === 'SELECT';
                if (!isOnForm) {
                    window.location.href = '/';
                }
            }
        });
    }

    // ===== Initialize on DOM Ready =====
    document.addEventListener('DOMContentLoaded', () => {
        ThemeManager.init();
        initAlerts();
        TaskActions.init();
        DragDropManager.init();
        initKeyboardShortcuts();
        
        // Add responsive labels to quadrants for mobile
        document.querySelectorAll('.quadrant-1').forEach(el => el.setAttribute('data-label', 'Q1: Urgent & Important - DO FIRST'));
        document.querySelectorAll('.quadrant-2').forEach(el => el.setAttribute('data-label', 'Q2: Not Urgent but Important - SCHEDULE'));
        document.querySelectorAll('.quadrant-3').forEach(el => el.setAttribute('data-label', 'Q3: Urgent but Not Important - DELEGATE'));
        document.querySelectorAll('.quadrant-4').forEach(el => el.setAttribute('data-label', 'Q4: Not Urgent & Not Important - ELIMINATE'));
    });

})();
