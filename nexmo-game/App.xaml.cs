using nexmo_game.Classes;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace nexmo_game
{
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        public static Methods methods;
        public static Player player;
        public static List<Country> countries;
        public static List<Carriers> carriers;
        public static List<Network> networks;
        public static Random random;
        public static List<LocalContact> contacts;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            random = new Random();
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // No repetir la inicialización de la aplicación si la ventana tiene contenido todavía,
            // solo asegurarse de que la ventana está activa.
            if (rootFrame == null)
            {
                // Crear un marco para que actúe como contexto de navegación y navegar a la primera página.
                rootFrame = new Frame();

                // TODO: Cambiar este valor a un tamaño de caché adecuado para la aplicación
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Cargar el estado de la aplicación suspendida previamente
                }

                // Poner el marco en la ventana actual.
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Quita la navegación de transición en el inicio.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // Cuando no se restaura la pila de navegación para navegar a la primera página,
                // configurar la nueva página al pasar la información requerida como parámetro
                // de navegación
                if (!rootFrame.Navigate(typeof(Pages.StartScreen), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Asegurarse de que la ventana actual está activa.
            Window.Current.Activate();
        }

        /// <summary>
        /// Restaura las transiciones de contenido después de iniciar la aplicación.
        /// </summary>
        /// <param name="sender">Objeto al que se asocia el controlador.</param>
        /// <param name="e">Detalles sobre el evento de navegación.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Se invoca al suspender la ejecución de la aplicación.  El estado de la aplicación se guarda
        /// sin saber si la aplicación se terminará o se reanudará con el contenido
        /// de la memoria aún intacto.
        /// </summary>
        /// <param name="sender">Origen de la solicitud de suspensión.</param>
        /// <param name="e">Detalles sobre la solicitud de suspensión.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Guardar el estado de la aplicación y detener toda actividad en segundo plano
            deferral.Complete();
        }
    }
}