import { AxiosInstance } from "axios";
import queryString from "query-string";
import { Result, Succeeded, Unauthorized, ValidationProblem } from "./results";
import {
  ChangeAccountForm,
  ChangePasswordForm,
  ConfirmAccountForm,
  CreateAccountForm,
  RefreshTokenForm,
  ResetPasswordForm,
  SendResetPasswordCodeForm,
  SignInForm,
  SignInWithProvider,
  SignOutForm,
  UserProfileModel,
  UserSessionModel
} from "./types";

export class IdentityService {
  constructor(private api: AxiosInstance) {}

  async createAccountAsync<Form extends CreateAccountForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded>(
      this.api.post("/identity/create", form)
    );
  }

  async confirmAccountAsync<Form extends ConfirmAccountForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded>(
      this.api.post("/identity/confirm", form)
    );
  }

  async changeAccountAsync<Form extends ChangeAccountForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Unauthorized | Succeeded>(
      this.api.post("/identity/change", form)
    );
  }

  async changePasswordAsync<Form extends ChangePasswordForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Unauthorized | Succeeded>(
      this.api.post("/identity/password/change", form)
    );
  }

  async sendResetPasswordCodeAsync<Form extends SendResetPasswordCodeForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded>(
      this.api.post("/identity/password/reset/send-code", form)
    );
  }

  async resetPasswordAsync<Form extends ResetPasswordForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded>(
      this.api.post("/identity/password/reset", form)
    );
  }

  async signInAsync<Form extends SignInForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded<UserSessionModel>>(
      this.api.post("/identity/sign-in", form)
    );
  }

  async SignInWithAsync(provider: SignInWithProvider, token: string) {
    return await Result.handle<ValidationProblem | Succeeded<UserSessionModel>>(
      this.api.post(`/identity/sign-in/${provider.toLowerCase()}/${token}`)
    );
  }

  signInWithRedirect(provider: SignInWithProvider, callbackUrl: string) {
    const redirectUrl = queryString.stringifyUrl({
      url: `${this.api.defaults.baseURL!.replace(/\/+$/, "")}/identity/sign-in/${provider.toLowerCase()}`,
      query: { callbackUrl }
    });
    return redirectUrl;
  }

  async refreshTokenAsync<Form extends RefreshTokenForm>(form: Form) {
    return await Result.handle<ValidationProblem<Form> | Succeeded<UserSessionModel>>(
      this.api.post("/identity/refresh-token", form)
    );
  }

  async signOutAsync<Form extends SignOutForm>(form: Form) {
    return await Result.handle<Unauthorized | Succeeded>(this.api.post("/identity/sign-out", form));
  }

  async getProfileAsync() {
    return await Result.handle<Unauthorized | Succeeded<UserProfileModel>>(
      this.api.get("/identity/profile")
    );
  }
}
