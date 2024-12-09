import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';

import { SampleComponent } from './sample/sample.component';

const APP_ROUTES: Routes = [
  { path: '', redirectTo: '/formRenderer/observations', pathMatch: 'full' },
  { path: 'formRenderer/observations', component: SampleComponent },
];


// Preloading lets router load lazy-loadable modules in the background while the user is interacting with application.
// This allows us to get the benefits of lazy-loading without losing the application like feel.
// export const appRouting = RouterModule.forRoot(APP_ROUTES, {preloadingStrategy: PreloadAllModules});

@NgModule({
  imports: [RouterModule.forRoot(APP_ROUTES, { useHash: true, preloadingStrategy: PreloadAllModules })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
