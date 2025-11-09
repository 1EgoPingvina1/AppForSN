import { Routes } from '@angular/router';
import { HomePageComponent } from './components/home-page/home-page.component';
import { LoginComponent } from './modules/auth/login/login.component';
import { authGuard } from './core/_guards/authGuard.guard';
import { AccountManagmentComponent } from './modules/profile/account-managment/account-managment.component';
import { ProjectCreateComponent } from './modules/projects/project-create/project-create.component';
import { SecuritySettingsComponent } from './modules/profile/security-settings/security-settings.component';
import { TwoFactorSetupComponent } from './modules/auth/two-factor/two-factor-setup/two-factor-setup.component';
import { TwoFactorVerifyComponent } from './modules/auth/two-factor/two-factor-verify/two-factor-verify.component';
import { GameLibraryComponent } from './modules/library/game-library/game-library.component';
import { ProjectDetailsComponent } from './modules/projects/project-details/project-details.component';

export const routes: Routes = [
    { path: '', component: HomePageComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children: [
            { path: 'create-project', component: ProjectCreateComponent },
            { path: 'my-account', component: AccountManagmentComponent },
            { path: 'security-settings', component: SecuritySettingsComponent },
            { path: 'library',component: GameLibraryComponent},
            { path: 'project-details/:id', component:ProjectDetailsComponent},
            {
                path: 'otp-setup',
                component: TwoFactorSetupComponent
            },
            {
                path: 'otp-verify',
                component: TwoFactorVerifyComponent
            },
            {path: '',component:GameLibraryComponent}
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: '' }
];
