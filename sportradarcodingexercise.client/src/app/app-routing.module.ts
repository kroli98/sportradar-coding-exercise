import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { EventListComponent } from './lists/event-list/event-list.component';
import { AddEventComponent } from './add-event/add-event.component';

const routes: Routes = [

  { path: '', redirectTo: '/events', pathMatch: 'full' },
  { path: 'events', component: EventListComponent },
  { path: 'add-event', component: AddEventComponent}


  ];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
