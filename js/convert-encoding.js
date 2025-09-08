// JS para Conversor de Codificación
// Mantenerlo simple y no intrusivo

(() => {
  const $ = (sel) => document.querySelector(sel);
  const baseInput = $('#basePath');
  const btnProyecto = $('#btn-ruta-proyecto');
  const btnExplorar = $('#btn-explorar');

  // Rellena con la ruta del proyecto detectada desde el propio HTML (meta simple: el valor que ya viene por defecto)
  if (btnProyecto && baseInput) {
    btnProyecto.addEventListener('click', () => {
      const defaultVal = baseInput.getAttribute('value') || baseInput.value;
      baseInput.value = defaultVal;
      baseInput.focus();
      baseInput.select();
    });
  }

  // Confirmación básica al enviar si la carpeta parece vacía o sospechosa
  const form = $('#form-conversor');
  if (form) {
    form.addEventListener('submit', (e) => {
      const ruta = baseInput ? baseInput.value.trim() : '';
      if (!ruta) {
        e.preventDefault();
        alert('Por favor, indique una carpeta base válida.');
        return;
      }
    });
  }

  // Modal explorador de carpetas
  const modal = $('#modal-picker');
  const pickerCerrar = $('#picker-cerrar');
  const pickerSubir = $('#picker-subir');
  const pickerSeleccionar = $('#picker-seleccionar');
  const pickerRutaActual = $('#picker-ruta-actual');
  const pickerLista = $('#picker-lista');
  const pickerMensaje = $('#picker-mensaje');

  let dirActual = '';

  function endpointListar(dir) {
    // Mismo script, endpoint JSON
    const base = window.location.pathname.replace(/[#?].*$/, '');
    const url = `${base}?listar=1&dir=${encodeURIComponent(dir || '')}`;
    return url;
  }

  async function cargarDirectorio(dir) {
    pickerMensaje.textContent = 'Cargando…';
    pickerLista.innerHTML = '';
    try {
      const res = await fetch(endpointListar(dir), { headers: { 'Accept': 'application/json' } });
      if (!res.ok) throw new Error('Error de red');
      const data = await res.json();
      if (!data.ok) throw new Error(data.mensaje || 'No se pudo listar');
      dirActual = data.directorio || dirActual;
      pickerRutaActual.value = dirActual || '';
      pickerMensaje.textContent = '';
      if (!data.subcarpetas || data.subcarpetas.length === 0) {
        pickerMensaje.textContent = 'No hay subcarpetas.';
      }
      const frag = document.createDocumentFragment();
      (data.subcarpetas || []).forEach((ruta) => {
        const li = document.createElement('li');
        const spanIcono = document.createElement('span');
        spanIcono.className = 'icono';
        spanIcono.textContent = '📁';
        const spanTexto = document.createElement('span');
        spanTexto.textContent = ruta;
        li.appendChild(spanIcono);
        li.appendChild(spanTexto);
        li.addEventListener('click', () => cargarDirectorio(ruta));
        frag.appendChild(li);
      });
      pickerLista.appendChild(frag);
      // Control del botón subir
      if (data.parent) {
        pickerSubir.disabled = false;
        pickerSubir.onclick = () => cargarDirectorio(data.parent);
      } else {
        pickerSubir.disabled = true;
        pickerSubir.onclick = null;
      }
    } catch (e) {
      pickerMensaje.textContent = e && e.message ? e.message : 'Error al listar';
    }
  }

  function abrirModal() {
    if (!modal) return;
    modal.classList.remove('oculto');
    modal.setAttribute('aria-hidden', 'false');
    const inicio = (baseInput && baseInput.value.trim()) || '';
    cargarDirectorio(inicio);
  }

  function cerrarModal() {
    if (!modal) return;
    modal.classList.add('oculto');
    modal.setAttribute('aria-hidden', 'true');
  }

  if (btnExplorar) {
    btnExplorar.addEventListener('click', abrirModal);
  }
  if (pickerCerrar) {
    pickerCerrar.addEventListener('click', cerrarModal);
  }
  if (modal) {
    modal.addEventListener('click', (e) => {
      if (e.target === modal) cerrarModal();
    });
  }
  if (pickerSeleccionar) {
    pickerSeleccionar.addEventListener('click', () => {
      if (baseInput) {
        baseInput.value = pickerRutaActual.value || '';
        baseInput.focus();
        baseInput.select();
      }
      cerrarModal();
    });
  }
})();
